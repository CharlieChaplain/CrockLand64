using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource intro;
    public AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        if (intro != null)
        {
            intro.volume = SoundManager.Instance.musicVolume;
            source.volume = SoundManager.Instance.musicVolume;

            intro.Play();
            float introLength = intro.clip.length;
            source.PlayDelayed(introLength);
        } else
        {
            source.volume = SoundManager.Instance.musicVolume;
            source.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(intro != null)
        {
            intro.volume = SoundManager.Instance.musicVolume;
        }
        source.volume = SoundManager.Instance.musicVolume;
    }
}
