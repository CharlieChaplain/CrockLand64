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
        //failsafe if music lists are empty
        if (intros.Length == 0 && sources.Length == 0)
            return;

        sourceIndex = 0;
        //starts all musics so they will be synced
        for (int i = 0; i < sources.Length; i++)
        {
            if (intros.Length > 0)
            {
                intros[i].Play();
                float introLength = intros[sourceIndex].clip.length;
                PDAInfo info = new PDAInfo(sources[i], introLength);
                //sources[i].PlayDelayed(introLength);
                StartCoroutine("PlayDelayedAlt", info);
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
        if (isFading || (intros.Length == 0 && sources.Length == 0))
            return;

        if (intros.Length > 0)
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
        if (index == sourceIndex)
            return;
        StartCoroutine(Fading(index));
    }

    public void Pause()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            if (intros != null)
                intros[i].Pause();
            sources[i].Pause();
        }
    }

    public void UnPause()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            if (intros != null)
                intros[i].UnPause();
            sources[i].UnPause();
        }
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
    /// <summary>
    /// used instead of base PlayDelayed because this accounts for pausing the game
    /// </summary>
    /// <param name="info">Custom class that contains the audio source to play, and the time to delay</param>
    IEnumerator PlayDelayedAlt(PDAInfo info)
    {
        yield return new WaitForSeconds(info.time);
        info.source.Play();
    }
}
/// <summary>
/// Used to pass information into the PlayDelayedAlt coroutine
/// </summary>
class PDAInfo
{
    public AudioSource source;
    public float time;

    public PDAInfo(AudioSource _s, float _t)
    {
        source = _s;
        time = _t;
    }
}
