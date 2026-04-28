using Unity.Cinemachine;
using UnityEngine;

public class LobbyCameraSwitcher : MonoBehaviour
{

    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CinemachineCamera _camera2;
   
  
    public void giveCameraOnePriority()
    {
        _camera.Priority = 1;
        _camera2.Priority = 0;
    }

    public void giveCameraTwoPriority()
    {
        _camera.Priority = 0;
        _camera2.Priority = 1;
    }
}
