using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
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
        StartCoroutine(KillAfterSeconds(objectToKill, timeTilKill));
    }

    IEnumerator KillAfterSeconds(GameObject objectToKill, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(objectToKill);
    }
}
