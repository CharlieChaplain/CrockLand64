using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Wealth
{
    [Header("To Disable")]
    public MeshRenderer mesh;

    protected override void ToggleModel(bool visibility)
    {
        mesh.enabled = visibility;
    }

    /*
    public ParticleSystem onCollectParticles;

    public int value;

    public PlaySound collectSound;

    [Header("To Disable")]
    public GameObject shadow;
    public ParticleSystem particles;
    public MeshRenderer mesh;
    Collider col;

    public bool gravityAffected;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9) //9 = player
        {
            PlayerManager.Instance.AddWealth(value);
            collectSound.Play(transform.position);
            DisableAll();
            onCollectParticles.Play();
            Destroy(this, 2f);
        }
    }

    void DisableAll()
    {
        shadow.SetActive(false);
        particles.Stop();
        particles.Clear();
        mesh.enabled = false;
        col.enabled = false;
    }
    */
}
