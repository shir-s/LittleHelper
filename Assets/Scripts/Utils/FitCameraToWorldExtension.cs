using UnityEngine;
using Unity.Cinemachine;

[DefaultExecutionOrder(100)]
public class FitCameraDirect : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private float worldWidth = 10f;

    void LateUpdate()
    {
        if (vcam == null) return;
        float aspect = (float)Screen.width / Screen.height;
        
        // Copy the struct, mutate it
        var lens = vcam.Lens;  
        lens.OrthographicSize = worldWidth / (2f * aspect);
        
        // Assign the whole struct back (NOT to OrthographicSize directly)
        vcam.Lens = lens;  
    }
}