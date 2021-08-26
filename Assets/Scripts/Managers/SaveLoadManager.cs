using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    public static void SaveTreasure(TreasureMaster treasure)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/crockFile.cl", FileMode.Create);

        SaveData data = new SaveData(treasure);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public static void LoadTreasure()
    {
        if(File.Exists(Application.persistentDataPath + "/crockFile.cl"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/crockFile.cl", FileMode.Open);

            SaveData data = bf.Deserialize(stream) as SaveData;

            stream.Close();

            TreasureMaster.Instance.LoadAllTreasure(data.wealth, data.treasures, data.moles);
        }
        else
            Debug.LogError("File of name " + Application.persistentDataPath + "/crockFile.cl" + " does not exist");
    }

    [Serializable]
    public class SaveData
    {
        public int wealth;
        public bool[,] treasures;
        public bool[,] moles;

        public SaveData(TreasureMaster treasure)
        {
            treasures = new bool[9, 5];
            moles = new bool[9, 5];

            wealth = treasure.wealth;

            for (int x = 0; x < 9; x++)
            {
                for(int y = 0; y < 5; y++)
                {

                    treasures[x, y] = treasure.QueryTreasure(x, y);
                    moles[x, y] = treasure.QueryMole(x, y);
                }
            }
        }
    }
}
