using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene_UnlockChest : Cutscene
{
    public GameObject chestCam;

    public GameObject cutsceneCrock;
    public GameObject cutsceneKey;
    public GameObject cutsceneCam;

    Animator crockAnim;
    Animator camAnim;
    Animator keyAnim;
    Animator chestAnim;

    bool canClose = false;


    private void Update()
    {
        if(canClose && Input.GetAxis("Punch") > 0)
        {
            canClose = false;
            StartCoroutine("WipeOutAndReload");
        }
    }

    private void Start()
    {
        crockAnim = cutsceneCrock.GetComponent<Animator>();
        keyAnim = cutsceneKey.GetComponent<Animator>();
        camAnim = cutsceneCam.GetComponent<Animator>();
        chestAnim = GetComponentInChildren<Animator>();
    }
    public override void PlayCutscene()
    {
        //Debug.Log("play unlock chest cutscene");

        GameObject player = PlayerManager.Instance.player;
        GameObject key = player.GetComponent<Attack>().GetCarryTarget();

        player.GetComponent<Attack>().matchTargetPos = false;
        PlayerManager.Instance.canMove = false;
        player.GetComponent<PlayerMove>().gravityMult = 0;

        cutsceneCrock.transform.position = transform.position;
        cutsceneKey.transform.position = transform.position;

        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<CharacterController>().transform.position = Vector3.zero;
        player.GetComponent<CharacterController>().enabled = true;

        key.transform.position = Vector3.zero;

        CameraManager.Instance.SetCamera(chestCam, 0);

        crockAnim.SetTrigger("Unlock");
        camAnim.SetTrigger("Unlock");
        keyAnim.SetTrigger("Unlock");
        chestAnim.SetTrigger("Unlock");

        canClose = true;
    }

    IEnumerator WipeOutAndReload()
    {
        IrisWipe.Instance.WipeOut();
        yield return new WaitForSeconds(3f);
        SceneDirector.ReloadScene();
    }
}
