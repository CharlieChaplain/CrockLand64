﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeModel : MonoBehaviour
{
    public List<SkinnedMeshRenderer> allCrockMeshes;

    SkinnedMeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    /// <summary>
    /// Changes crock's model to the specified index
    /// </summary>
    /// <param name="index">0 = regular; 1 = separated teeth; 2 = stone form</param>
    public void ChangeModelTo(int index)
    {
        mesh.sharedMesh = allCrockMeshes[index].sharedMesh;

        mesh.sharedMaterials = allCrockMeshes[index].sharedMaterials;
    }
}
