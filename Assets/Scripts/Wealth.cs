using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wealth : MonoBehaviour
{
    public ParticleSystem onCollectParticles;

    [Header("To Disable")]
    public GameObject shadow;
    public ParticleSystem particles;

    public int value;

    public PlaySound collectSound;

    public bool gravityAffected;
    public float killTimer; //how long the doubloon/gem will take before disappearing. if == -1, then it never disappears
    protected bool visible;
    protected bool blinkOn = false;

    public Collider crockCol;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().useGravity = gravityAffected;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9 && other.GetComponent<Hurt>() && !other.GetComponent<Hurt>().GetInvincible()) //9 = player
        {
            killTimer = -1f;

            PlayerManager.Instance.AddWealth(value);
            collectSound.Play(transform.position);
            DisableAll();
            onCollectParticles.Play();
            Destroy(this, 2f);
        }
    }

    private void Update()
    {
        if (killTimer > 0)
        {
            killTimer -= Time.deltaTime;
            if (killTimer < 3f && !blinkOn)
                StartCoroutine("blink");
        }
    }


    protected void DisableAll()
    {
        shadow.SetActive(false);
        particles.Stop();
        particles.Clear();
        ToggleModel(false);
        GetComponent<SphereCollider>().enabled = false;
        crockCol.enabled = false;
    }

    protected virtual void ToggleModel(bool visibility) { }

    IEnumerator blink()
    {
        blinkOn = true;
        while (killTimer > 0)
        {
            visible = !visible;
            ToggleModel(visible);
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
