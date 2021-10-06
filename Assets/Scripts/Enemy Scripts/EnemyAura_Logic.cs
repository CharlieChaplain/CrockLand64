using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAura_Logic : MonoBehaviour
{
    public Material mat;
    public ParticleSystem shockwavePart;

    Enemy_Pursue pursue;
    CapsuleCollider col;
    float timeOffset; //is set when fadein is done, so the sinusoidal alpha fade starts at 1
    bool fade;

    //call this when aura is spawned
    public void OnSpawn()
    {
        StartCoroutine(Appear());
        pursue = GetComponent<Enemy_Pursue>();
        pursue.target = PlayerManager.Instance.player;
        col = GetComponent<CapsuleCollider>();
        col.enabled = false;
        fade = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fade)
        {
            float alpha = (Mathf.Sin(Time.time - timeOffset) * .25f) + 0.75f;
            mat.color = new Color(1f, 1f, 1f, alpha);
        }

        Vector3 velocity = pursue.Pursue() * Mathf.Min(1f, (pursue.target.transform.position - transform.position).magnitude);
        transform.position += velocity * Time.deltaTime;
    }

    /// <summary>
    /// call this when the aura actually affects crock
    /// </summary>
    public void Activate()
    {
        col.enabled = true;
        fade = false;
        shockwavePart.Play();
        StartCoroutine(Disappear());
    }

    IEnumerator Appear()
    {
        float timeToAppear = 0.5f;
        float alphaMult = 1f / timeToAppear;
        for(float f = 0; f < timeToAppear; f += Time.deltaTime)
        {
            mat.color = new Color(1f, 1f, 1f, f * alphaMult);
            yield return null;
        }
        timeOffset = Time.time;
        fade = true;
    }
    IEnumerator Disappear()
    {
        float timeToDisappear = 0.2f;
        float alphaMult = 1f / timeToDisappear;
        float initScale = transform.localScale.x;
        for (float f = 0; f < timeToDisappear; f += Time.deltaTime)
        {
            mat.color = new Color(1f, 1f, 1f, 1f - (f * alphaMult));
            float newScale = initScale + (f * alphaMult * 1.5f);
            transform.localScale = new Vector3(initScale, newScale, initScale);

            yield return null;
        }
        mat.color = new Color(1f, 1f, 1f, 0);

        col.enabled = false;
        Destroy(gameObject, 0.5f);
    }
}
