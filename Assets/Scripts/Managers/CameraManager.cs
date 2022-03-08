using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public CinemachineVirtualCameraBase currentCamera;
    public Camera sceneCam;

    public void OnSceneLoad()
    {
        sceneCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        currentCamera = GameObject.Find("CrockCam").GetComponent<CinemachineVirtualCameraBase>();
    }

    public void SetCamera(GameObject newCam, float transitionTime)
    {
        if(currentCamera != null)
            currentCamera.Priority = 5;

        if (currentCamera.GetComponent<CinemachineCollider>())
            currentCamera.GetComponent<CinemachineCollider>().enabled = false;

        currentCamera = newCam.GetComponent<CinemachineVirtualCameraBase>();
        currentCamera.Priority = 10;

        sceneCam.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = transitionTime;

        if (currentCamera.GetComponent<CinemachineCollider>())
            currentCamera.GetComponent<CinemachineCollider>().enabled = true;
    }
}
