using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    //used to populate the dictionary, can fill them in in inspector
    public List<int> allCutscenesNumbers;
    public List<Cutscene> allCutscenesCutscenes;
    public static Dictionary<int, Cutscene> allCutscenes;


    public static CutsceneManager Instance { get; private set; }

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

        allCutscenes = new Dictionary<int, Cutscene>();

        if (allCutscenesNumbers.Count != allCutscenesCutscenes.Count)
        {
            Debug.Log("numbers and cutscenes lists do not match");
            return;
        }

        for (int i = 0; i < allCutscenesNumbers.Count; i++)
        {
            allCutscenes.Add(allCutscenesNumbers[i], allCutscenesCutscenes[i]);
        }
    }

    public static void PlayCutscene(int index)
    {
        allCutscenes[index].PlayCutscene();
    }
}
