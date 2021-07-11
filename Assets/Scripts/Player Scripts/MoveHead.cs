using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHead : MonoBehaviour
{
    //this script moves crocks head anchor to always be at head height in world space, not local space. This object is what the camera
    //looks at, so the camera won't snap around a bunch when his local up is changed.

    public GameObject target;
    public Vector3 offset;

    Vector3 reference;
    public float followSpeed;


    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + offset, ref reference, followSpeed);
    }
}
