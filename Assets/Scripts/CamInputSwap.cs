using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamInputSwap : MonoBehaviour
{
    private CinemachineFreeLook freeLook;

    // Start is called before the first frame update
    void Start()
    {
        freeLook = GetComponent<CinemachineFreeLook>(); //must be placed on the same script as the cinemachine free look
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.keyboardInput)
        {
            freeLook.m_XAxis.m_InputAxisName = "Mouse X";
            freeLook.m_YAxis.m_InputAxisName = "Mouse Y";
        }
        else if (!InputManager.Instance.keyboardInput)
        {
            freeLook.m_XAxis.m_InputAxisName = "HorizontalLook";
            freeLook.m_YAxis.m_InputAxisName = "VerticalLook";
        }
    }
}
