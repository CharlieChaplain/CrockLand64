using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDynamic : MonoBehaviour
{
    public GameObject shadowPrefab;
    public float scale = 1f; //how big or small to scale this entity's shadows
    GameObject[] casters;
    List<GameObject> shadows;

    void Start()
    {
        casters = GameObject.FindGameObjectsWithTag("ShadowCaster");
        shadows = new List<GameObject>();
        for (int i = 0; i < casters.Length; i++)
        {
            GameObject newShadow = Instantiate(shadowPrefab);
            newShadow.AddComponent<ShadowMove>();
            newShadow.GetComponent<ShadowMove>().target = transform;
            newShadow.transform.localScale *= scale;
            shadows.Add(newShadow);
        }
    }

    void LateUpdate()
    {
        for(int i = 0; i < casters.Length; i++)
        {
            Vector3 direction = casters[i].transform.position - transform.position;
            float distance = direction.magnitude;
            float lightRadius = casters[i].GetComponent<ShadowCaster>().lightRadius;
            float opacity = (lightRadius - distance) / lightRadius;
            if(distance < lightRadius)
            {
                GameObject shadow = shadows[i]; 
                shadow.GetComponent<MeshRenderer>().enabled = true;
                Vector3 shadowUp = shadow.transform.up;
                shadow.transform.rotation = Quaternion.LookRotation(direction, shadowUp);
                Quaternion rot = Quaternion.FromToRotation(shadow.transform.up, shadowUp);
                shadow.transform.rotation = rot * shadow.transform.rotation;

                shadow.GetComponent<MeshRenderer>().materials[0].color = new Color(1f, 1f, 1f, Mathf.Sqrt(opacity));
            }
            else
            {
                shadows[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
