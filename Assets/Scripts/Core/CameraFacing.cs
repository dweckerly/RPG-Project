using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        Camera mainCamera;

        private void Awake() 
        {
            mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            transform.forward = mainCamera.transform.forward;
        }
    }
}

