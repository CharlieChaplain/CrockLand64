using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script will turn off colliders of geometry when crock is in ghost form
public class GhostFence : MonoBehaviour
{
    Collider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.Instance.currentForm == PlayerManager.PlayerForm.ghost)
            col.enabled = false;
        else
            col.enabled = true;
    }
}
