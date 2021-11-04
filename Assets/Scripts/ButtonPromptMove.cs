using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPromptMove : MonoBehaviour
{
    float degRot;
    float initScale;

    public float oscSpeed;

    private void Start()
    {
        initScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        float sinAmt = Mathf.Sin(Time.time * oscSpeed) * 0.1f;
        float cosAmt = Mathf.Cos(Time.time * oscSpeed) * 0.1f;
        float scale = (1f + cosAmt) * initScale;
        transform.localScale = new Vector3(scale, scale, scale);
        transform.localRotation = Quaternion.Euler(0, sinAmt * 360f, 0);
    }
}
