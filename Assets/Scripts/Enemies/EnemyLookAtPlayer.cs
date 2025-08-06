using System;
using Managers;
using UnityEngine;

namespace Enemies
{
    public class EnemyLookAtTarget : MonoBehaviour
    {
        private Transform _target;
        [SerializeField] private float _angleOffset = 0f;

        private void Awake()
        {
            _target = GameManager.Instance.PlayerObject.transform;
        }

        private void Update()
        {
            Vector2 direction = _target.position - transform.position;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            transform.rotation = Quaternion.Euler(0f, 0f, angle + _angleOffset);
        }
    }
}