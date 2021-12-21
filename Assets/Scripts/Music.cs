using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource[] intros;
    public AudioSource[] sources;
    //---------AS OF RIGHT NOW, the Swim script on crock will change the music to index 1 when going underwater. idk what to do
    //---------if a level does not have water in it yet, will figure that out when the problem arises

    int sourceIndex; //the index of the current track to play. Changes as crock moves to different areas with different music

    bool isFading = false;

    // Start is called before the first frame update
    void Start()
    {
        sourceIndex = 0;
        //starts all musics so they will be synced
        for (int i = 0; i < sources.Length; i++)
        {
            if (intros != null)
            {
                intros[i].Play();
                float introLength = intros[sourceIndex].clip.length;
                sources[i].PlayDelayed(introLength);
            }
            else
            {
                sources[i].Play();
            }
        }
        if (intros != null)
        {
            intros[sourceIndex].volume = SoundManager.Instance.musicVolume;
        }
        sources[sourceIndex].volume = SoundManager.Instance.musicVolume;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
            return;

        if (intros != null)
        {
            intros[sourceIndex].volume = SoundManager.Instance.musicVolume;
        }
        sources[sourceIndex].volume = SoundManager.Instance.musicVolume;
        
    }

    public int GetSourceIndex()
    {
        return sourceIndex;
    }

    public void ChangeMusic(int index)
    {
        StartCoroutine(Fading(index));
    }

    IEnumerator Fading(int newIndex)
    {
        isFading = true;
        float time = 0;
        float maxTime = 2f; //time for the transition to take
        while(time < maxTime)
        {
            if (intros != null)
            {
                intros[sourceIndex].volume = Mathf.Lerp(SoundManager.Instance.musicVolume, 0, time / maxTime);
                intros[newIndex].volume = Mathf.Lerp(0, SoundManager.Instance.musicVolume, time / maxTime);
            }
            sources[sourceIndex].volume = Mathf.Lerp(SoundManager.Instance.musicVolume, 0, time / maxTime);
            sources[newIndex].volume = Mathf.Lerp(0, SoundManager.Instance.musicVolume, time / maxTime);
            time += Time.deltaTime;
            yield return null;
        }
        if (intros != null)
        {
            intros[sourceIndex].volume = 0;
            intros[newIndex].volume = SoundManager.Instance.musicVolume;
        }
        sources[sourceIndex].volume = 0;
        sources[newIndex].volume = SoundManager.Instance.musicVolume;

        sourceIndex = newIndex;
        isFading = false;
    }
}
