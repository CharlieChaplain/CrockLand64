using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Key_Logic.TreasureColor chestColor;

    public GameObject treasure;

    BoxCollider closedCollision;
    BoxCollider openCollision;

    public bool opened;
    //bool playCutscene = false;

    // Start is called before the first frame update
    void Start()
    {
        closedCollision = transform.Find("closedCollision").GetComponent<BoxCollider>();
        openCollision = transform.Find("openCollision").GetComponent<BoxCollider>();
    }

    void Open()
    {
        GetComponent<Cutscene>().PlayCutscene();
        closedCollision.enabled = false;
        openCollision.enabled = true;

        opened = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Key_Logic>() != null && other.GetComponent<Key_Logic>().keyColor == chestColor && !opened)
        {
            Open();
        }
    }
}
