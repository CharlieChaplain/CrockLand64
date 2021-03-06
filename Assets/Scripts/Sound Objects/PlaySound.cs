using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Sounds/PlaySound Object")]
public class PlaySound : ScriptableObject
{
    public AudioClip sound;
    protected GameObject soundObj;

    public float volume;
    public float distance = 30f; //the distance at which you can start hearing the sound
    public bool loop;

    //used for looping sounds
    private bool playing = false;

    /// <param name="position">The position at which the sound is played</param>
    public virtual void Play(Vector3 position)
    {
        playing = true;
        soundObj = new GameObject();
        soundObj.transform.position = position;
        soundObj.AddComponent<AudioSource>();
        soundObj.GetComponent<AudioSource>().clip = sound;
        soundObj.GetComponent<AudioSource>().volume = SoundManager.Instance.soundEffectVolume;
        soundObj.GetComponent<AudioSource>().loop = loop;
        soundObj.GetComponent<AudioSource>().maxDistance = distance;
        soundObj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
        soundObj.GetComponent<AudioSource>().spatialBlend = 1f;
        soundObj.GetComponent<AudioSource>().Play();
        if (loop)
            SoundManager.Instance.loopingSounds.Add(this);
        else
            SoundManager.Instance.KillSoundObject(soundObj, soundObj.GetComponent<AudioSource>().clip.length + 0.1f);
    }
    //Use when the sound object needs to be parented to a gameobject
    public virtual void Play(Transform parent)
    {
        playing = true;
        soundObj = new GameObject();
        soundObj.transform.parent = parent;
        soundObj.transform.position = parent.position;
        soundObj.AddComponent<AudioSource>();
        soundObj.GetComponent<AudioSource>().clip = sound;
        soundObj.GetComponent<AudioSource>().volume = SoundManager.Instance.soundEffectVolume;
        soundObj.GetComponent<AudioSource>().loop = loop;
        soundObj.GetComponent<AudioSource>().maxDistance = distance;
        soundObj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
        soundObj.GetComponent<AudioSource>().spatialBlend = 1f;
        soundObj.GetComponent<AudioSource>().Play();
        if (loop)
            SoundManager.Instance.loopingSounds.Add(this);
        else
            SoundManager.Instance.KillSoundObject(soundObj, soundObj.GetComponent<AudioSource>().clip.length + 0.1f);
    }

    public virtual void Play(Vector3 position, float delay)
    {
        playing = true;
        soundObj = new GameObject();
        soundObj.transform.position = position;
        soundObj.AddComponent<AudioSource>();
        soundObj.GetComponent<AudioSource>().clip = sound;
        soundObj.GetComponent<AudioSource>().volume = SoundManager.Instance.soundEffectVolume;
        soundObj.GetComponent<AudioSource>().loop = loop;
        soundObj.GetComponent<AudioSource>().maxDistance = distance;
        soundObj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
        soundObj.GetComponent<AudioSource>().spatialBlend = 1f;
        soundObj.GetComponent<AudioSource>().PlayDelayed(delay);
        if (loop)
            SoundManager.Instance.loopingSounds.Add(this);
        else
            SoundManager.Instance.KillSoundObject(soundObj, soundObj.GetComponent<AudioSource>().clip.length + 0.1f);
    }
    //Use when the sound object needs to be parented to a gameobject
    public virtual void Play(Transform parent, float delay)
    {
        playing = true;
        soundObj = new GameObject();
        soundObj.transform.parent = parent;
        soundObj.transform.position = parent.position;
        soundObj.AddComponent<AudioSource>();
        soundObj.GetComponent<AudioSource>().clip = sound;
        soundObj.GetComponent<AudioSource>().volume = SoundManager.Instance.soundEffectVolume;
        soundObj.GetComponent<AudioSource>().loop = loop;
        soundObj.GetComponent<AudioSource>().maxDistance = distance;
        soundObj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
        soundObj.GetComponent<AudioSource>().spatialBlend = 1f;
        soundObj.GetComponent<AudioSource>().PlayDelayed(delay);
        if (loop)
            SoundManager.Instance.loopingSounds.Add(this);
        else
            SoundManager.Instance.KillSoundObject(soundObj, soundObj.GetComponent<AudioSource>().clip.length + 0.1f);
    }

    public virtual void Stop()
    {
        playing = false;
        //stop doesn't work for non looping sounds
        if (!loop)
            return;
        SoundManager.Instance.loopingSounds.Remove(this);
        SoundManager.Instance.KillSoundObject(soundObj, 0);
    }

    public virtual void ChangeVol(float vol)
    {
        soundObj.GetComponent<AudioSource>().volume = vol;
    }

    public virtual bool IsPlaying()
    {
        return playing;
    }
}
