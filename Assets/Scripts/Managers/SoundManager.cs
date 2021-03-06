using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float musicVolume; //set by options menu, sent to sound objects when they play music
    public float soundEffectVolume; //set by options menu, sent to sound objects when they play sound effects

    public List<PlaySound> loopingSounds; //gets added to by any looping playsounds so they can be accessed to change volume

    public Music music;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnSceneLoad()
    {
        music = GameObject.Find("Main Camera/Music").GetComponent<Music>();
    }

    public void KillSoundObject(GameObject objectToKill, float timeTilKill)
    {
        Destroy(objectToKill, timeTilKill);
    }

    public void changeLoopSoundsVolume()
    {
        foreach(PlaySound SO in loopingSounds)
        {
            SO.ChangeVol(soundEffectVolume);
        }
    }

    public void ChangeMusic(int index)
    {
        music.ChangeMusic(index);
    }
}
