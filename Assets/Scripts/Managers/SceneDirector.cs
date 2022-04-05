using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    public static SceneDirector Instance { get; private set; }

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine("WaitToFadeIn");

        //-----------------Do all On Scene Load stuff here---------------------

        CameraManager.Instance.OnSceneLoad();
        PlayerManager.Instance.OnSceneLoad();
        IrisWipe.Instance.OnSceneLoad();
        SoundManager.Instance.OnSceneLoad();
        TreasureMaster.Instance.OnSceneLoad();
        InputManager.Instance.OnSceneLoad();
        DialogueManager.Instance.OnSceneLoad();
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    //will wait a small amount of time so that things can load properly
    IEnumerator WaitToFadeIn()
    {
        yield return new WaitForSeconds(2f);
        IrisWipe.Instance.WipeIn();
        PlayerManager.Instance.canMove = true;
    }
}
