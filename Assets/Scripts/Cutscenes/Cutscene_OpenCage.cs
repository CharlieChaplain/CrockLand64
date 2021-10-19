using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene_OpenCage : Cutscene
{
    public GameObject cutsceneCam;
    GameObject prevCam;

    public TargetTarget cage;

    public override void PlayCutscene()
    {
        prevCam = CameraManager.Instance.currentCamera.gameObject;
        StartCoroutine(Things());
    }

    IEnumerator Things()
    {
        PlayerManager.Instance.canMove = false;
        CameraManager.Instance.SetCamera(cutsceneCam, 2f);

        yield return new WaitForSeconds(2f);

        cage.GetComponent<TargetTarget>().anim.SetTrigger("Open");

        yield return new WaitForSeconds(2f);

        CameraManager.Instance.SetCamera(prevCam, 2f);

        PlayerManager.Instance.canMove = true;
    }
}
