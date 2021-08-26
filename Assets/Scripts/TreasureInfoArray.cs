using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TreasureInfoArray
{
    [System.Serializable]
    public struct columnData
    {
        public TreasureInfo[] treasure;
    }

    public columnData[] levels = new columnData[9];

    public TreasureInfo this[int level, int treasure]
    {
        get
        {
            return levels[level].treasure[treasure];
        }
    }
}
