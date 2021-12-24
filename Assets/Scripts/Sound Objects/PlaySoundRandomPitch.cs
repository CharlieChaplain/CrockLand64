using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Sounds/PlaySound Object RandomPitch")]
public class PlaySoundRandomPitch : PlaySound
{
    public float pitchVariance;

    /// <param name="position">The position at which the sound is played</param>
    public override void Play(Vector3 position)
    {
        soundObj = new GameObject();
        soundObj.transform.position = position;
        soundObj.AddComponent<AudioSource>();
        soundObj.GetComponent<AudioSource>().clip = sound;
        soundObj.GetComponent<AudioSource>().volume = SoundManager.Instance.soundEffectVolume;
        soundObj.GetComponent<AudioSource>().loop = loop;
        soundObj.GetComponent<AudioSource>().maxDistance = 30f;
        soundObj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
        soundObj.GetComponent<AudioSource>().spatialBlend = 1f;
        soundObj.GetComponent<AudioSource>().pitch = Random.Range(1f - pitchVariance, 1f + pitchVariance);
        soundObj.GetComponent<AudioSource>().Play();
        SoundManager.Instance.KillSoundObject(soundObj, soundObj.GetComponent<AudioSource>().clip.length + 0.1f);
    }
}
