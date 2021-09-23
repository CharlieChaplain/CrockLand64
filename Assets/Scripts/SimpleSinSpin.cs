using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSinSpin : MonoBehaviour
{
    public Vector3 spinSpeed;
    public Vector3 spinMagnitude;

    Vector3 initRot;

    private void Start()
    {
        initRot = transform.rotation.eulerAngles;
    }

    void Update()
    {
        Vector3 newRotEulers = new Vector3(
            Mathf.Sin(Time.time * spinSpeed.x) * spinMagnitude.x,
            Mathf.Sin(Time.time * spinSpeed.y) * spinMagnitude.y,
            Mathf.Sin(Time.time * spinSpeed.z) * spinMagnitude.z
            );

        transform.rotation = Quaternion.Euler(initRot + newRotEulers);
    }
}
