using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explorer_Logic : MonoBehaviour
{
    public float maxSpeed;
    public Transform home; //where she wanders around
    public float wanderRadius; //how far she will wander before angling her direction inward
    float speed;
    bool talking;

    Vector3 direction;

    float timer;

    // Start is called before the first frame update
    void Start()
    {
        talking = false;
        timer = Random.Range(2f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Move()
    {

    }
}
