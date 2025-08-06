using Managers;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

namespace Door
{
    public class RoomTransition : MonoBehaviour
    {
        public CinemachineCamera previousCamera;
        public CinemachineCamera newCamera;
    
        public void SwitchCamera()
        {
            StartCoroutine(SwitchCameraCoroutine());
        }

        private IEnumerator SwitchCameraCoroutine()
        {
            if (previousCamera != null)
            {
                previousCamera.LookAt = null;
                previousCamera.Follow = null;
                previousCamera.Priority = 0;
            }

            yield return null;

            if (newCamera != null && GameManager.Instance.PlayerObject != null)
            {
                Transform playerTransform = GameManager.Instance.PlayerObject.transform;
                newCamera.LookAt = playerTransform;
                newCamera.Follow = playerTransform;
                newCamera.Priority = 1;
            }
            else
            {
                Debug.LogWarning("PlayerObject is null or newCamera is null during camera switch!");
            }
        }
    }
}