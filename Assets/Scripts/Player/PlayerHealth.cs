using System;
using System.Collections;
using Managers;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        
        [SerializeField] internal int maxHealth = 3;
        private int _currentHealth;
        //public event Action<int, int> OnHealthChanged;

        [SerializeField] private PlayerStats stats;
    
        private PlayerAnimatorController _animatorController;
        private bool _isInvincible = false;
        private bool _isDead = false;
        
        [Header("Invincibility")]
        [SerializeField] private float invincibilityDuration = 0.75f;
        [SerializeField] private float flashDelay = 0.0833f;

        private void Awake()
        {
            _animatorController = GetComponent<PlayerAnimatorController>();
        }

        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.Space))
        //     {
        //         TakeDamage(1);
        //     }
        // }

        private void Start()
        {
            if (stats != null)
            {
                maxHealth = stats.maxHealth;
            }

            _currentHealth = maxHealth;
            //OnHealthChanged?.Invoke(currentHealth, maxHealth);
            GameEvents.PlayerHealthChanged?.Invoke(_currentHealth, maxHealth);
        }
        private void OnEnable()
        {
            GameEvents.RestartLevel += HandleRestart;
        }

        private void OnDisable()
        {
            GameEvents.RestartLevel -= HandleRestart;
        }

        private void HandleRestart()
        {
            _isDead        = false; 
            _currentHealth = maxHealth;
            //OnHealthChanged?.Invoke(currentHealth, maxHealth);
            GameEvents.PlayerHealthChanged?.Invoke(_currentHealth, maxHealth);
            _isInvincible = false;
        }
        public void TakeDamage(int amount)
        {
            if (_isInvincible || _isDead) return;
            SoundManager.Instance.PlayHit();
            TriggerInvincibility(invincibilityDuration);
            _animatorController.SetHurt();
            _currentHealth -= amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
            Debug.Log($"Player took {amount} damage. Current health: {_currentHealth}");
            //OnHealthChanged?.Invoke(currentHealth, maxHealth);
            GameEvents.PlayerHealthChanged?.Invoke(_currentHealth, maxHealth);
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void TriggerInvincibility(float duration)
        {
            if (duration > 0f)
            {
                // start one coroutine that both gates damage
                // and drives the flashing on the animator
                StartCoroutine(InvincibilityRoutine(duration));
            }
        }
        
        private IEnumerator InvincibilityRoutine(float duration)
        {
            _isInvincible = true;
            float elapsed = 0f;
            bool   visible = true;

            while (elapsed < duration)
            {
                // toggle sprite visibility
                visible = !visible;
                _animatorController.SetSpritesVisible(visible);

                // wait for the flash interval
                yield return new WaitForSeconds(flashDelay);
                elapsed += flashDelay;
            }

            // ensure we’re fully visible & vulnerable again
            _animatorController.SetSpritesVisible(true);
            _isInvincible = false;
        }
        
        
    
        public int GetHealth()
        {
            return _currentHealth;
        }

        private void Die()
        {
            _isDead = true; 
            Debug.Log("Player died!");
            //SoundManager.Instance?.PlayGameOver();
            GameEvents.SetUpDeath?.Invoke();
        }

        public void Heal(int amount)
        {
            _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);
            //OnHealthChanged?.Invoke(currentHealth, maxHealth);
            GameEvents.PlayerHealthChanged?.Invoke(_currentHealth, maxHealth);
            // גם פה אפשר לעדכן UI
        }
    
        public void IncreaseMaxHealth(int amount)
        {
            maxHealth += amount;
            _currentHealth += amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
            //OnHealthChanged?.Invoke(currentHealth, maxHealth);
            GameEvents.PlayerHealthChanged?.Invoke(_currentHealth, maxHealth);
            Debug.Log($"[Health] Extra health purchased! MaxHealth: {maxHealth}, CurrentHealth: {_currentHealth}");
        }
    
        public void SetInvincible(bool value)
        {
            _isInvincible = value;
        }
    }
}