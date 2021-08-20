using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureMaster : MonoBehaviour
{
    public static TreasureMaster Instance { get; private set; }

    bool[,] moleArray;

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

    // Start is called before the first frame update
    void Start()
    {
        //instantiate the mole array
        moleArray = new bool[9, 5];
        for(int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                moleArray[i, j] = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveMole(int level, int mole)
    {
        moleArray[level, mole] = true;
    }
    public bool QueryMole(int level, int mole)
    {
        return moleArray[level, mole];
    }
}
