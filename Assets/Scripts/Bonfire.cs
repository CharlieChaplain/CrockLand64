using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour
{
    public PlaySound fireSound;
    // Start is called before the first frame update
    void Start()
    {
        fireSound.Play(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
