using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public List<Texture> eyeTextures;
    public Material eyeMaterial;

    private float TimeBetweenBlinks;

    void Start()
    {
        TimeBetweenBlinks = Random.Range(3f, 5f);
    }

    void Update()
    {
        TimeBetweenBlinks -= Time.deltaTime;

        if(TimeBetweenBlinks <= 0)
        {
            TimeBetweenBlinks = Random.Range(3f, 5f);
            StartCoroutine("BlinkCorout");
        }
    }

    IEnumerator BlinkCorout()
    {
        for(int index = 0; index < eyeTextures.Count; index++)
        {
            eyeMaterial.SetTexture("_MainTex", eyeTextures[index]);
            yield return new WaitForSeconds(0.1f);
        }
        
    }
}
