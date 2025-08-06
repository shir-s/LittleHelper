using UnityEngine;

namespace Enemies
{
    public class MoleAnimationEvents : MonoBehaviour
    {
        private MoleEnemy _moleEnemy;

        private void Awake()
        {
            _moleEnemy = GetComponentInParent<Enemies.MoleEnemy>();
            if (_moleEnemy == null)
            {
                Debug.LogError("MoleEnemy not found in parent objects!");
            }
        }

        public void Pause()
        {
            if (_moleEnemy != null)
                _moleEnemy.StartCoroutine(_moleEnemy.PauseAnimation());
        }
        
        public void OnAnimationComplete(string animationName)
        {
            if (_moleEnemy != null)
                _moleEnemy.NotifyAnimationEnded(animationName);
        }

    }
}