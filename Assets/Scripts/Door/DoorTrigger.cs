using UnityEngine;

namespace Door
{
    public class DoorTrigger : MonoBehaviour
    {
        public Transform targetPosition;
        private RoomTransition _transition;

        private bool _playerInside;

        private void Awake()
        {
            _transition = GetComponent<RoomTransition>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_playerInside && other.CompareTag("Player"))
            {
                other.transform.parent.position = targetPosition.position;
                _transition.SwitchCamera();
                _playerInside = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = false;
            }
        }
    }
}
