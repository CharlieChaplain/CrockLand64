using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtTest : MonoBehaviour
{
    public float power;
    public int intensity;
    public int variance;

    public PlaySound onHurtSound;
    public ParticleSystem onHurtPart; //pull in whatever appropriate particle system is in the world on each occasion. For fire, use crock's sliding smoke particles
    public float timeToStopParticles; //whenever the particle system should stop playing

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9) //9 = player
        {
            Hurt playerHurt = other.GetComponent<Hurt>();
            if (playerHurt.GetInvincible())
                return;
            other.GetComponent<Hurt>().HurtPlayer(power, intensity, variance, transform.position);
            if (onHurtSound)
                onHurtSound.Play(transform.position);
            if (onHurtPart)
            {
                onHurtPart.Play();
                StartCoroutine(StopParticles(timeToStopParticles));
            }
        }
    }

    IEnumerator StopParticles(float time)
    {
        yield return new WaitForSeconds(time);
        onHurtPart.Stop();
    }
}
