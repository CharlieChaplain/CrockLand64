using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadHelperDebg : MonoBehaviour
{
    public bool LoadOnStart = false;

    // Start is called before the first frame update
    void Start()
    {
        if (LoadOnStart)
        {
            TreasureMaster.Instance.Load();
        }
    }
}
