using System.Collections;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

public class FreezingRoomEnemy : MonoBehaviour, IEnemy
{
    [Tooltip("Make sure that player stats has the same base time")]
    [SerializeField] private float baseFreezeTime = 30f;
    [SerializeField] private float reducedTime = 3f;

    private float _currentTime;
    private bool _playerInside = false;
    private bool _hasAttacked = false;
    private GameObject _player;
    private Coroutine _freezeCoroutine;
    private int _damage = 1;
    private int _attackInterval = 2;

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
        OnRoundStarted(1);
        GameEvents.OnTimerVisibilityChanged?.Invoke(false);
        StopFreezeCoroutine();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // SoundManager.Instance.PlayFreezerAmbience();
        /*var stats = other.GetComponentInParent<PlayerController>()?.stats;
        if (stats == null)
        {
            Debug.LogError("Missing stats!");
            return;
        }

        //_player = other.gameObject;
        baseFreezeTime = stats.TotalFreezeTime;*/
        _hasAttacked = false;
        _playerInside = true;

        GameEvents.OnTimerVisibilityChanged?.Invoke(true);
        GameEvents.OnTimerUpdated?.Invoke(_currentTime);
        GameEvents.OnFreezeStarted?.Invoke(_currentTime);

        StopFreezeCoroutine();
        _freezeCoroutine = StartCoroutine(FreezeCountdown(other.gameObject));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // SoundManager.Instance.StopAmbience();
        _playerInside = false;
        GameEvents.OnTimerVisibilityChanged?.Invoke(false);

        StopFreezeCoroutine();

        var stats = other.GetComponent<PlayerController>()?.stats;
        if (stats != null) _currentTime = stats.TotalFreezeTime;
    }
    
    private IEnumerator FreezeCountdown(GameObject player)
    {
        bool inAttackLoop = false;
        PlayerHealth playerHealth = player.GetComponentInParent<PlayerHealth>();
        while (_playerInside)
        {
            if (playerHealth.GetHealth() <= 0)
            {
                GameEvents.OnTimerVisibilityChanged?.Invoke(false);
                break;
            }
            
            _currentTime -= Time.deltaTime;
            GameEvents.OnTimerUpdated?.Invoke(_currentTime);

            if (_currentTime <= 0f)
            {
                if (!inAttackLoop)
                {
                    AttackPlayer(player);
                    inAttackLoop = true;

                    GameEvents.OnTimerColorChanged?.Invoke(Color.red);

                    _currentTime = _attackInterval;
                }
                else
                {
                    if (player.GetComponentInParent<PlayerHealth>().GetHealth() > 0)
                    {
                        AttackPlayer(player);

                        _currentTime = _attackInterval;
                    }
                    else
                    {
                        GameEvents.OnTimerVisibilityChanged?.Invoke(false);
                        break;
                    }
                }
            }
            else if (!inAttackLoop)
            {
                GameEvents.OnTimerColorChanged?.Invoke(Color.white);
            }

            yield return null;
        }

        _freezeCoroutine = null;
    }



    private void StopFreezeCoroutine()
    {
        if (_freezeCoroutine != null)
        {
            StopCoroutine(_freezeCoroutine);
            _freezeCoroutine = null;
        }
    }

    public void AttackPlayer(GameObject player)
    {
        var health = player.GetComponentInParent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(_damage); 
        }
        else
        {
            Debug.LogWarning("Player has no PlayerHealth!");
        }
    }

    public void OnRoundStarted(int level)
    {
        var player = GameManager.Instance.PlayerObject;
        var stats = player?.GetComponent<PlayerController>()?.stats;
        if (stats == null)
        {
            Debug.LogWarning("Player stats not found!");
            return;
        }

        baseFreezeTime = stats.TotalFreezeTime;
        _currentTime = Mathf.Max(3f, baseFreezeTime - (level - 1) * reducedTime);
    }

}
