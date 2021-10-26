using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float musicVolume; //set by options menu, sent to sound objects when they play music
    public float soundEffectVolume; //set by options menu, sent to sound objects when they play sound effects

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

    public void KillSoundObject(GameObject objectToKill, float timeTilKill)
    {
        Destroy(objectToKill, timeTilKill);
    }
}
