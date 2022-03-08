using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InWorldSound : MonoBehaviour
{
    public PlaySound sound;
    bool doOffset;
    // Start is called before the first frame update
    void Start()
    {
        float offset = 0;
        if (doOffset)
        {
            offset = Random.Range(0, 1f);
        }
        sound.Play(transform, offset);
    }
}
