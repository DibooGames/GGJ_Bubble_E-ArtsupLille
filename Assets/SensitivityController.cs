using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SensitivityController : MonoBehaviour
{

    public float sensitivity;
    public CinemachineVirtualCamera VirtualCamera;





    void Start()
    {
        VirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    // Update is called once per frame
    void Update()
    {
        VirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = sensitivity;
        VirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = sensitivity;

    }
}
