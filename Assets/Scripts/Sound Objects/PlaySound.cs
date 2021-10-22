using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Sounds/PlaySound Object")]
public class PlaySound : ScriptableObject
{
    public AudioClip sound;
    protected GameObject soundObj;

    public float volume;

    /// <param name="position">The position at which the sound is played</param>
    public virtual void Play(Vector3 position)
    {
        soundObj = new GameObject();
        soundObj.transform.position = position;
        soundObj.AddComponent<AudioSource>();
        soundObj.GetComponent<AudioSource>().clip = sound;
        soundObj.GetComponent<AudioSource>().volume = SoundManager.Instance.soundEffectVolume;
        soundObj.GetComponent<AudioSource>().Play();
        SoundManager.Instance.KillSoundObject(soundObj, soundObj.GetComponent<AudioSource>().clip.length + 0.1f);
    }
}
