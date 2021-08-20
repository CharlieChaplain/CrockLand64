using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoleUI : MonoBehaviour
{
    public bool saved = false;
    public string moleID;

    public List<Sprite> moleSprites; //0 is trapped, 1 is saved
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        if (!int.TryParse(moleID, out int errorCatcher))
            Debug.Log("Mole UI: MOLE ID " + moleID + " IS INVALID AND WILL THROW ERRORS");
        else
            saved = TreasureMaster.Instance.QueryMole(int.Parse(moleID.Substring(0, 1)), int.Parse(moleID.Substring(1, 1)));

        image = GetComponent<Image>();
        
        if (!saved)
        {
            image.sprite = moleSprites[0];
            image.color = new Color(.5f, .5f, .5f);
        }
        else
        {
            image.sprite = moleSprites[1];
            image.color = Color.white;
        }
    }

    public void SaveMole()
    {
        saved = true;
        image.sprite = moleSprites[1];
        image.color = Color.white;
    }
}
