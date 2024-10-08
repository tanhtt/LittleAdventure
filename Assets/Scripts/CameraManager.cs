using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    [Header("Camera Distance")]
    [SerializeField] private bool canChangeCameraDistance;
    private float targetCameraDistance;
    [SerializeField] private float distanceChangeRate;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if(!canChangeCameraDistance) return;

        float currentCamDistance = transposer.m_CameraDistance;

        // Avoid continue update a little camera distance
        if (Mathf.Abs(currentCamDistance - targetCameraDistance) < 0.1f) return;

        transposer.m_CameraDistance = Mathf.Lerp(currentCamDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void SetCameraDistance(float cameraDistance)
    {
        targetCameraDistance = cameraDistance;
    }


}
