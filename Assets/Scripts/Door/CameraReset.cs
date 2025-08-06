using Managers;
using Unity.Cinemachine;
using UnityEngine;
using Utils;

namespace Door
{
    public class CameraReset : MonoBehaviour
    {
        private CinemachineCamera _kitchenCamera;
        
        private void Awake()
        {
            _kitchenCamera = GetComponent<CinemachineCamera>();
        }
        private void OnEnable()
        {
            GameEvents.RestartLevel += ResetCamera;
        }

        
        private void OnDisable()
        {
            GameEvents.RestartLevel -= ResetCamera;
        }

        private void ResetCamera()
        {
            _kitchenCamera.Priority = 1;
            _kitchenCamera.LookAt = GameManager.Instance.PlayerObject.transform;
            _kitchenCamera.Follow = GameManager.Instance.PlayerObject.transform;
        }

        
    }
}