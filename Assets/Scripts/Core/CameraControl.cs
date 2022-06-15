using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace RPG.Core
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] float rotationSpeed = 100f;
        [SerializeField] float zoomSpeed = 30f;

        [Range(0, 50)]
        [SerializeField] float minDistance = 4f;
        [Range(0, 50)]
        [SerializeField] float maxDistance = 22f;

        void Update()
        {
            if (Input.GetKey(KeyCode.A)) 
                transform.RotateAround(target.position, new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * rotationSpeed);
            if (Input.GetKey(KeyCode.D)) 
                transform.RotateAround(target.position, new Vector3(0.0f, -1.0f, 0.0f), Time.deltaTime * rotationSpeed);
            // May implement up and down rotation but need to restrict input to one at a time and clamping.
            // if (Input.GetKey(KeyCode.W))
            //     transform.RotateAround(target.position, new Vector3(1.0f, 0.0f, 0.0f), Time.deltaTime * rotationSpeed);
            // if (Input.GetKey(KeyCode.S))
            //     transform.RotateAround(target.position, new Vector3(-1.0f, 0.0f, 0.0f), Time.deltaTime * rotationSpeed);
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                CinemachineComponentBase componentBase = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent(CinemachineCore.Stage.Body);
                if (componentBase is CinemachineFramingTransposer)
                {
                    float dist = (componentBase as CinemachineFramingTransposer).m_CameraDistance;
                    dist = Mathf.Clamp(dist - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minDistance, maxDistance);
                    (componentBase as CinemachineFramingTransposer).m_CameraDistance = dist;
                }
            }
        }
    }
}

