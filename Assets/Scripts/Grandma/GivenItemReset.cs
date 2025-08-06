
using UnityEngine;
using Utils;

namespace Grandma
{
    public class GivenItemReset : MonoBehaviour
    {
        [SerializeField] private Transform givenItemsContainer;

        private void OnEnable()
        {
            GameEvents.RestartLevel += ResetContainer;
        }

        private void OnDisable()
        {
            GameEvents.RestartLevel -= ResetContainer;
        }

        private void ResetContainer()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}