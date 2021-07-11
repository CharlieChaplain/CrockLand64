using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyPP : MonoBehaviour
{
    public Material PPMat;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        source.filterMode = FilterMode.Point;
        Graphics.Blit(source, destination, PPMat);
    }
}
