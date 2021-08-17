using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEulerSinusoidal : MonoBehaviour
{
    public Vector3 speed;
    public Vector3 amplitude;

    Vector3 initPos;

    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = initPos + new Vector3(amplitude.x * Mathf.Sin(speed.x * Time.time),
                                                        amplitude.y * Mathf.Sin(speed.y * Time.time),
                                                        amplitude.z * Mathf.Sin(speed.z * Time.time));
    }
}
