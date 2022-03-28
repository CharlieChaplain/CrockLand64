using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyAnimListener : MonoBehaviour
{
    public void SetFelled(int felled)
    {
        this.transform.root.GetComponent<Key_Logic>().SetFelled(felled == 1);
    }
}
