using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureMaster : MonoBehaviour
{
    public static TreasureMaster Instance { get; private set; }

    public TreasureList treasureList;

    bool[,] moleArray;

    public int wealth;
    public int displayWealth;

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
        treasureList = GameObject.Find("TreasureList").GetComponent<TreasureList>();

        //instantiate the mole array
        moleArray = new bool[9, 5];
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                moleArray[x, y] = false;
                if(treasureList.TIArray[x, y])
                    treasureList.TIArray[x, y].collected = false;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log(QueryTreasure(0, 0));
        }
    }

    public void AddWealth(int amount)
    {
        wealth += amount;
    }

    public void RescueMole(int level, int mole)
    {
        moleArray[level, mole] = true;
    }
    public bool QueryMole(int level, int mole)
    {
        return moleArray[level, mole];
    }

    public void CollectTreasure(int level, int treasure)
    {
        treasureList.TIArray[level, treasure].collected = true;
    }

    public bool QueryTreasure(int level, int treasure)
    {
        if(treasureList.TIArray[level, treasure])
            return treasureList.TIArray[level, treasure].collected;
        return false;
    }

    public void Save()
    {
        SaveAllTreasure();
    }
    public void Load()
    {
        SaveLoadManager.LoadTreasure();
    }

    /// <summary>
    /// saves all treasures, current wealth, and rescued moles
    /// </summary>
    public void SaveAllTreasure()
    {
        SaveLoadManager.SaveTreasure(this);
    }
    public void LoadAllTreasure(int _wealth, bool[,] _treasures, bool[,] _moles)
    {
        wealth = _wealth;
        displayWealth = wealth;

        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                moleArray[x, y] = _moles[x, y];
                if (treasureList.TIArray[x, y])
                    treasureList.TIArray[x, y].collected = _treasures[x, y];
            }
        }
    }
}
