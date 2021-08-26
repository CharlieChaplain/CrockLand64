using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Key_Logic.TreasureColor chestColor;

    public GameObject treasure;

    public Vector2 chestID; //the level and number of chest this is, for saving purposes
    public TreasureSlot treasureSlot; //the UI element for this treasure

    BoxCollider closedCollision;
    BoxCollider openCollision;

    public bool opened;
    //bool playCutscene = false;

    // Start is called before the first frame update
    void Start()
    {
        closedCollision = transform.Find("closedCollision").GetComponent<BoxCollider>();
        openCollision = transform.Find("openCollision").GetComponent<BoxCollider>();

        if (TreasureMaster.Instance.QueryTreasure(0, 0))
        {
            GetComponentInChildren<Animator>().SetBool("Opened", true);
            opened = true;
        }
    }

    void Open()
    {
        GetComponent<Cutscene>().PlayCutscene();
        closedCollision.enabled = false;
        openCollision.enabled = true;

        opened = true;

        TreasureMaster.Instance.CollectTreasure((int)chestID.x, (int)chestID.y);
        TreasureMaster.Instance.Save();
        treasureSlot.CollectTreasure();

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Key_Logic>() != null && other.GetComponent<Key_Logic>().keyColor == chestColor && !opened)
        {
            Open();
        }
    }
}
