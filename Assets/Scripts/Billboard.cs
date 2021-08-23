using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool billboardX;
    public bool billboardY;
    public bool billboardZ;

    public Camera cam;
    public Vector3 initRotation;

    void Start()
    {
        //this wont work if camera is not called main camera
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotationEulers = Vector3.zero;

        if (billboardX)
            rotationEulers.x = cam.transform.eulerAngles.x;
        if(billboardY)
            rotationEulers.y = cam.transform.eulerAngles.y;
        if (billboardZ)
            rotationEulers.z = cam.transform.eulerAngles.z;

        transform.rotation = Quaternion.Euler(rotationEulers + initRotation);
    }
}
