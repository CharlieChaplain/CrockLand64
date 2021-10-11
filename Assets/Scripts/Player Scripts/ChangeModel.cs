using System.Collections;
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

    /// <summary>
    /// Changes the texture of the material at the specified index
    /// </summary>
    /// <param name="index">The index in the mesh of the material to change</param>
    /// <param name="newTex">the texture to changet to</param>
    public void ChangeTexture(int index, Texture newTex)
    {
        mesh.materials[index].mainTexture = newTex;
    }
}
