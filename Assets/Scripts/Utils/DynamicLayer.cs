using UnityEngine;
using Managers;

namespace Utils
{
    public class DynamicLayer : MonoBehaviour
    {
        [SerializeField] private float minDeltaY = 0.1f; 
        private int layerOffset = 2070;  

        private SpriteRenderer[] _sprites;
        private int[] _originalOrders;
        private Transform _playerTransform;
        private bool _isAbove;

        private void Awake()
        {
            _playerTransform = GameManager.Instance.playerObject.transform;

            _sprites = GetComponentsInChildren<SpriteRenderer>(true);
            _originalOrders = new int[_sprites.Length];

            for (int i = 0; i < _sprites.Length; i++)
            {
                _originalOrders[i] = _sprites[i].sortingOrder;
            }

            _isAbove = false;
        }

        private void Update()
        {
            float deltaY = _playerTransform.position.y - transform.position.y;

            if (Mathf.Abs(deltaY) < minDeltaY)
                return;

            bool shouldBeAbove = deltaY > 0;

            if (shouldBeAbove == _isAbove)
                return;

            for (int i = 0; i < _sprites.Length; i++)
            {
                _sprites[i].sortingOrder = _originalOrders[i] + (shouldBeAbove ? layerOffset : 0);
            }

            _isAbove = shouldBeAbove;
        }
    }
}