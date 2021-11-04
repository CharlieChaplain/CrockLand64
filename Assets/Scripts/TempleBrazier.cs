using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleBrazier : MonoBehaviour
{
    public Mesh[] bothMeshes;
    public PlaySound igniteSound;
    public PlaySound flameSound;
    private Light flameLight;
    private ParticleSystem flamePart;
    private bool lit;

    // Start is called before the first frame update
    void Start()
    {
        flameLight = GetComponentInChildren<Light>();
        flamePart = GetComponentInChildren<ParticleSystem>();
        lit = false;
    }

    public void Light()
    {
        if (lit)
            return;
        lit = true;
        GetComponent<MeshFilter>().mesh = bothMeshes[1];
        flameLight.intensity = 3f;
        flamePart.Play();
        igniteSound.Play(flamePart.gameObject.transform.position);
        flameSound.Play(flamePart.gameObject.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9) //9 = player
        {
            Light();
        }
    }
}
