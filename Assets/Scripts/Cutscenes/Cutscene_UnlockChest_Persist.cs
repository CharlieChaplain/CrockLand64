using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This version of the unlock chest cutscene allows crock to stay in the level
/// </summary>
public class Cutscene_UnlockChest_Persist : Cutscene
{
    public GameObject chestCam;
    GameObject prevCam; //the camera used before the cutscene started

    public GameObject cutsceneCrock;
    public GameObject cutsceneKey;
    public GameObject cutsceneCam;
    GameObject treasure;

    Animator crockAnim;
    Animator camAnim;
    Animator keyAnim;
    Animator chestAnim;
    Animator treasureAnim;

    bool canClose = false;

    float oldGrav;

    public GameObject temple;


    private void Update()
    {
        if (canClose && Input.GetAxis("Punch") > 0)
        {
            canClose = false;
            StartCoroutine("WipeOut");
        }
    }

    private void Start()
    {
        crockAnim = cutsceneCrock.GetComponent<Animator>();
        keyAnim = cutsceneKey.GetComponent<Animator>();
        camAnim = cutsceneCam.GetComponent<Animator>();
        chestAnim = GetComponentInChildren<Animator>();

        treasure = GetComponent<Chest>().treasure;
        treasureAnim = treasure.GetComponent<Animator>();
    }
    public override void PlayCutscene()
    {
        Pause.activatePause = false;

        GameObject player = PlayerManager.Instance.player;
        GameObject key = player.GetComponent<Attack>().GetCarryTarget();
        player.GetComponent<Attack>().ResetCarryTarget();
        PlayerManager.Instance.canMove = false;
        oldGrav = player.GetComponent<PlayerMove>().gravityMult;
        player.GetComponent<PlayerMove>().gravityMult = 0;

        cutsceneCrock.transform.position = transform.position;
        cutsceneKey.transform.position = transform.position;

        player.SetActive(false);
        key.SetActive(false);

        prevCam = CameraManager.Instance.currentCamera.gameObject;
        CameraManager.Instance.SetCamera(chestCam, 0);

        crockAnim.SetTrigger("Unlock");
        camAnim.SetTrigger("Unlock");
        keyAnim.SetTrigger("Unlock");
        chestAnim.SetTrigger("Unlock");
        treasureAnim.SetTrigger("Unlock");

        canClose = true;
    }

    IEnumerator WipeOut()
    {
        IrisWipe.Instance.WipeOut();
        yield return new WaitForSeconds(3f);

        cutsceneCrock.SetActive(false);
        cutsceneKey.SetActive(false);
        cutsceneCam.SetActive(false);
        treasure.SetActive(false);

        GameObject player = PlayerManager.Instance.player;
        player.SetActive(true);
        PlayerManager.Instance.canMove = true;
        player.GetComponent<PlayerMove>().gravityMult = oldGrav;
        CameraManager.Instance.SetCamera(prevCam, 0);

        //starts cutscene of temple opening
        temple.SendMessage("PlayCutscene");
    }
}
