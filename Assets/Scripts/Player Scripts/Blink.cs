using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public List<Texture> eyeTextures;
    public Material eyeMaterial;

    private float TimeBetweenBlinks;

    bool eyesOpen = true;

    void Start()
    {
        TimeBetweenBlinks = Random.Range(3f, 5f);
    }

    void Update()
    {
        if(eyesOpen)
            TimeBetweenBlinks -= Time.deltaTime;

        if(TimeBetweenBlinks <= 0)
        {
            TimeBetweenBlinks = Random.Range(3f, 5f);
            StartCoroutine("BlinkCorout");
        }
    }

    public void CloseEyes()
    {
        eyesOpen = false;
        eyeMaterial.SetTexture("_MainTex", eyeTextures[1]);
    }
    public void OpenEyes()
    {
        eyesOpen = true;
        eyeMaterial.SetTexture("_MainTex", eyeTextures[0]);
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
