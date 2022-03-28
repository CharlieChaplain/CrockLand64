using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAnimListener : MonoBehaviour
{
    Transform bodyPos;

    private void Start()
    {
        bodyPos = transform.root.GetComponent<Dummy_Logic>().bodyPos;
    }
    public void MovePos()
    {
        transform.root.position = bodyPos.position;
    }
}
