using Managers;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UIElements;
using Utils;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerStats stats;
        private Rigidbody2D _rb;
        private Vector2 _movement;
        private PlayerAnimatorController _animatorController;

        // Hide upgrade variables
        private bool isHiding = false;
        private bool canHide = true;
        private float currentHideCooldown = 0f;
        private PlayerHealth playerHealth;
        
        private Vector3 _firstLocation;
        private AudioSource footstepSource;
        private bool isMovingSoundPlaying = false;


        private void Awake()
        {
            GameManager.Instance.playerObject = gameObject;
            _firstLocation = gameObject.transform.position;
            _rb = GetComponent<Rigidbody2D>();
            playerHealth = GetComponentInChildren<PlayerHealth>();
            _animatorController = GetComponent<PlayerAnimatorController>();
            
            //sound
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.clip = SoundManager.Instance.footstep;
            footstepSource.loop = true;
            footstepSource.volume = 2f;
        }
        
        private void OnEnable()
        {
            GameEvents.RestartLevel += HandleRestart;
            GameEvents.SetUpDeath += DisableScript;
        }

        // private void OnDisable()
        // {
        //     GameEvents.RestartLevel -= HandleRestart;
        //     GameEvents.RestartLevel -= DisableScript;
        // }

        private void HandleRestart()
        {
            enabled = true;
            _rb.linearVelocity = Vector2.zero; 
            gameObject.transform.position = _firstLocation;
        }
        
        private void DisableScript()
        {
            _movement = Vector2.zero;
            _rb.linearVelocity = Vector2.zero; 
            enabled = false;
        }

        private void Update()
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");

            if (_movement.magnitude > 0.1f)
            {
                if (!isMovingSoundPlaying)
                {
                    footstepSource.Play();
                    isMovingSoundPlaying = true;
                }
                _animatorController.SetWalking(true);
                if (Mathf.Abs(_movement.x) > Mathf.Abs(_movement.y))
                {
                    _animatorController.ActivateModel(ModelType.Side);
                    var isMovingRight = _movement.x > 0;
                    _animatorController.SetSideDirectionRight(isMovingRight);
                }
                else
                {
                    var isMovingUp = _movement.y > 0;
                    _animatorController.ActivateModel(isMovingUp ? ModelType.Back: ModelType.Front);
                }
            }
            else
            {
                _animatorController.SetWalking(false);
                footstepSource.Stop();
                isMovingSoundPlaying = false;

            }

            // HIDE input
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (!stats.hasHideUpgrade)
                {
                    Debug.Log("[PlayerController] Tried to use Hide but upgrade not purchased!");
                    return;
                }
                if (canHide)
                {
                    Debug.Log($"[Time: {Time.time:F2}] Pressed H — starting HideRoutine (Duration: {stats.hideDuration} seconds)");
                    StartCoroutine(HideRoutine());
                }
                else
                {
                    Debug.Log($"[Time: {Time.time:F2}] Tried to hide but still in cooldown! Time left: {currentHideCooldown:F2}s");
                }
            }
            
            // Cooldown update
            if (!canHide)
            {
                currentHideCooldown -= Time.deltaTime;
                if (currentHideCooldown <= 0f)
                {
                    canHide = true;
                    Debug.Log($"[{Time.time:F2}] Hide is ready again.");
                }
            }
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _movement.normalized * stats.moveSpeed;
        }
        

        private IEnumerator HideRoutine()
        {
            if (isHiding)
            {
                Debug.Log("Hide is already active — ignoring repeated activation.");
                yield break;
            }

            Debug.Log("Started HideRoutine");

            isHiding = true;

            Debug.Log($"[{Time.time:F2}] Hide activated for {stats.hideDuration} seconds!");

            _animatorController.SetTransparency(0.4f);

            // Set invincibility
            if (playerHealth != null)
            {
                playerHealth.SetInvincible(true);
            }
            else
            {
                Debug.LogError("[PlayerController] PlayerHealth reference is null during Hide!");
            }

            yield return new WaitForSeconds(stats.hideDuration);
            
            _animatorController.SetTransparency(1f);

            if (playerHealth != null)
            {
                playerHealth.SetInvincible(false);
            }

            Debug.Log("Hide effect has ended — player is visible and vulnerable again.");

            // Resetting cooldown
            canHide = false;
            currentHideCooldown = stats.hideCooldown;
            isHiding = false;
        }
    }
}

