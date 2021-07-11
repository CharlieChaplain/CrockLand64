using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doubloon : MonoBehaviour
{
    public ParticleSystem onCollectParticles;

    [Header("To Disable")]
    public GameObject shadow;
    public ParticleSystem particles;
    public SpriteRenderer sprite;
    public Collider col;

    public PlaySound collectSound;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            PlayerManager.Instance.AddWealth(1);
            collectSound.Play(transform.position);
            DisableAll();
            onCollectParticles.Play();
            StartCoroutine("Kill");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisableAll()
    {
        shadow.SetActive(false);
        particles.Stop();
        particles.Clear();
        sprite.enabled = false;
        col.enabled = false;
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this);
    }
}
