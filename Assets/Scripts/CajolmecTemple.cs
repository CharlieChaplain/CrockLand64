using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CajolmecTemple : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetBool("Opened", TreasureMaster.Instance.QueryTreasure(0, 0));
    }
}
