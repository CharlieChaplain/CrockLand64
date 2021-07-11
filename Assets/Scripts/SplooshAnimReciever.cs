using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplooshAnimReciever : MonoBehaviour
{
    // Start is called before the first frame update
    public void Kill()
    {
        Destroy(this.gameObject);
    }
}
