using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene_UnlockTemple : Cutscene
{
    public GameObject templeCam;

    public GameObject idol;
    public GameObject camAnchor;
    public GameObject temple;

    GameObject prevCam;

    Animator idolAnim;
    Animator camAnim;
    Animator templeAnim;

    float oldGrav;


    private void Start()
    {
        idolAnim = idol.GetComponent<Animator>();
        camAnim = camAnchor.GetComponent<Animator>();
        templeAnim = temple.GetComponent<Animator>();
    }
    public override void PlayCutscene()
    {
        Pause.activatePause = false;

        GameObject player = PlayerManager.Instance.player;

        player.SetActive(false);
        PlayerManager.Instance.canMove = false;
        oldGrav = player.GetComponent<PlayerMove>().gravityMult;
        player.GetComponent<PlayerMove>().gravityMult = 0;

        prevCam = CameraManager.Instance.currentCamera.gameObject;
        CameraManager.Instance.SetCamera(templeCam, 0);

        StartCoroutine("Unlock");
    }

    IEnumerator Unlock()
    {
        IrisWipe.Instance.WipeIn();

        yield return new WaitForSeconds(1f);

        idolAnim.SetTrigger("Unlock");
        camAnim.SetTrigger("Unlock");
        templeAnim.SetTrigger("Unlock");

        yield return new WaitForSeconds(13f);

        IrisWipe.Instance.WipeOut();
        yield return new WaitForSeconds(3f);

        idol.SetActive(false);

        GameObject player = PlayerManager.Instance.player;
        player.SetActive(true);
        PlayerManager.Instance.canMove = true;
        player.GetComponent<PlayerMove>().gravityMult = oldGrav;
        CameraManager.Instance.SetCamera(prevCam, 0);

        IrisWipe.Instance.WipeIn();

        Pause.activatePause = true;
    }
}
