using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class RatEnemy : MonoBehaviour, IEnemy
    {
        public float moveSpeed = 2f;
        private Vector2 _randomDirection;
        private float changeDirectionTime = 2f;
        private float _timeUntilChange;
        private Rigidbody2D _rb;
        
        private void OnEnable()
        {
            _timeUntilChange = 0f; 
            _randomDirection = Vector2.zero;
            transform.rotation = Quaternion.identity;

            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();
    
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }


        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>(); 
            _rb.gravityScale = 0;
            _rb.freezeRotation = true;
            PickNewDirection();
        }

        private void Update()
        {
            //MoveRandomly();

            _timeUntilChange -= Time.deltaTime;
            if (_timeUntilChange <= 0f)
            {
                PickNewDirection();
            }
        }

        // private void MoveRandomly()
        // {
        //     transform.Translate(randomDirection * (moveSpeed * Time.deltaTime));
        // }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _randomDirection * moveSpeed;
        }

        private void PickNewDirection()
        {
            _randomDirection = Random.insideUnitCircle.normalized;
            _timeUntilChange = changeDirectionTime;
            RotateToDirection(_randomDirection);
        }

        private void RotateToDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                AttackPlayer(other.gameObject);
                _randomDirection *= -1; 
                _timeUntilChange = changeDirectionTime;
            }

            if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Door") || other.gameObject.layer == LayerMask.NameToLayer("Container") || other.gameObject.layer == LayerMask.NameToLayer("Decore") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // Reflect the direction when hitting a wall
                _randomDirection = Vector2.Reflect(_randomDirection, other.contacts[0].normal);
                RotateToDirection(_randomDirection);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                AttackPlayer(other.gameObject);
            }
        }

        public void AttackPlayer(GameObject player)
        {
            var health = player.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(1); 
            }
        }

        public void OnRoundStarted(int level)
        {
            moveSpeed = 5f + level * 0.5f;
        }
    }
}