using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeBubbleCollider : MonoBehaviour
{
    GameObject waterVolume;
    BoxCollider thisCol;

    // Start is called before the first frame update
    void Start()
    {
        thisCol = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (waterVolume != null)
            Resize();
    }

    public void Resize()
    {
        if (waterVolume != null)
        {
            thisCol.center = waterVolume.transform.position - transform.position;
            Vector3 size = waterVolume.transform.localScale;
            size.y -= 0.5f;
            thisCol.size = size;

            thisCol.transform.rotation = Quaternion.identity;
        }
    }

    public void SetWaterVolume(GameObject _waterVolume)
    {
        waterVolume = _waterVolume;
    }

    public void UnsetWaterVolume()
    {
        waterVolume = null;
        thisCol.center = Vector3.zero;
        thisCol.size = new Vector3(0.1f, 0.1f, 0.1f);
    }
}
