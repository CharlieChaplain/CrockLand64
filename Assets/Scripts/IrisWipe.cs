using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class IrisWipe : MonoBehaviour
{
    public static IrisWipe Instance { get; private set; }

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

    public GameObject irisWipe;

    public TimelineAsset TL_wipeIn;
    public TimelineAsset TL_wipeOut;


    public void OnSceneLoad()
    {
        irisWipe = GameObject.Find("Canvas/IrisWipe");
    }

    public void WipeIn()
    {
        irisWipe.GetComponent<PlayableDirector>().playableAsset = TL_wipeIn;
        irisWipe.GetComponent<PlayableDirector>().Play();
    }

    public void WipeOut()
    {
        irisWipe.GetComponent<PlayableDirector>().playableAsset = TL_wipeOut;
        irisWipe.GetComponent<PlayableDirector>().Play();
    }
}
