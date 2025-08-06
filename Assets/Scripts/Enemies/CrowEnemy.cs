using Managers;
using Player;
using UnityEngine.UIElements;

namespace Enemies
{
    using UnityEngine;
    using System.Collections;

    public class CrowEnemy : MonoBehaviour, IEnemy
    {
        [SerializeField] private float flightHeight = 3f;
        [SerializeField] private float followSpeed = 2f;
        [SerializeField] private float diveSpeed = 6f;
        [SerializeField] private float timeBetweenDives = 5f;
        [SerializeField] private float returnSpeed = 4f;
        [SerializeField] private GameObject shadowPrefab;
        [SerializeField] private Vector3 shadowMinScale = new Vector3(0.5f, 0.2f, 1f);
        [SerializeField] private Vector3 shadowMaxScale = new Vector3(1.2f, 0.5f, 1f);

        private GameObject player;
        private GameObject shadow;
        private Vector3 originalOffset;
        private float diveCooldown;
        private bool isDiving = false;
        private bool isReturning = false;
        private Vector3 diveShadowPosition;

        private void Start()
        {
            player = GameManager.Instance.playerObject;
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Rigidbody2D>().freezeRotation = true;
            originalOffset = new Vector3(0, flightHeight, 0);
            diveCooldown = timeBetweenDives;

            shadow = Instantiate(shadowPrefab, transform.position - originalOffset, Quaternion.identity);
            shadow.transform.localScale = shadowMinScale;
        }

        private void Update()
        {
            if (player == null) return;

            diveCooldown -= Time.deltaTime;

            if (!isDiving && !isReturning)
            {
                FollowPlayer();

                if (diveCooldown <= 0f)
                {
                    StartCoroutine(DiveAttack());
                }

                Vector3 shadowPos = new Vector3(transform.position.x, transform.position.y - flightHeight, shadow.transform.position.z);
                shadow.transform.position = shadowPos;
                shadow.transform.localScale = shadowMinScale;
            }
            else
            {
                shadow.transform.position = diveShadowPosition;

                float distance = Vector2.Distance(transform.position, diveShadowPosition);
                float t = 1f - Mathf.Clamp01(distance / flightHeight);
                shadow.transform.localScale = Vector3.Lerp(shadowMinScale, shadowMaxScale, t);
            }

            Vector3 scaleCorrection = new Vector3(
                (shadow.transform.localScale.x - shadowMinScale.x) / 2f,
                0,
                0
            );
            shadow.transform.position -= scaleCorrection;
        }

        private void FollowPlayer()
        {
            Vector3 target = player.transform.position + originalOffset;
            transform.position = Vector3.MoveTowards(transform.position, target, followSpeed * Time.deltaTime);
        }

        private IEnumerator DiveAttack()
        {
            isDiving = true;
            diveShadowPosition = new Vector3(player.transform.position.x, player.transform.position.y, shadow.transform.position.z);

            while (Vector3.Distance(transform.position, diveShadowPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, diveShadowPosition, diveSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            isDiving = false;
            isReturning = true;
        }

        private void FixedUpdate()
        {
            if (isReturning)
            {
                Vector3 returnTarget = player.transform.position + originalOffset;
                transform.position = Vector3.MoveTowards(transform.position, returnTarget, returnSpeed * Time.fixedDeltaTime);

                if (Vector3.Distance(transform.position, returnTarget) < 0.1f)
                {
                    isReturning = false;
                    diveCooldown = timeBetweenDives;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") && (isDiving || isReturning))
            {
                AttackPlayer(other.gameObject);
            }
            if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Door"))
            {
                if (!isDiving && !isReturning)
                {
                    Vector3 collisionNormal = other.contacts[0].normal;
                    Vector3 reflectDirection = Vector3.Reflect((player.transform.position - transform.position).normalized, collisionNormal);
                    Vector3 bounceTarget = transform.position + reflectDirection * 0.5f;

                    transform.position = Vector3.MoveTowards(transform.position, bounceTarget, followSpeed * Time.deltaTime);
                }
                else if (isDiving)
                {
                    StopAllCoroutines();
                    isDiving = false;
                    isReturning = true;
                }
            }
        }

        public void AttackPlayer(GameObject player)
        {
            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(1);
            }
        }

        public void OnRoundStarted(int level)
        {
            timeBetweenDives = Mathf.Max(1f, timeBetweenDives - level * 0.5f);
        }
    }
}
