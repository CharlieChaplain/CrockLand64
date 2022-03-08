using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class NPC : MonoBehaviour
{
    //public CinemachineVirtualCameraBase talkCutsceneCamera; //the camera to lerp to 
    public CinemachineVirtualCameraBase originalCam;
    public Transform crockSpot; //where crock will move to stand in.
    public LayerMask groundMask;

    Animator anim;

    public Dialogue dialogue;

    public bool canEngage; //only flips to false directly after disengaging so crock doesn't immediately reengage

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        canEngage = true;
        RaycastHit hit;
        if (Physics.Raycast(crockSpot.position + (Vector3.up * 3f), Vector3.down, out hit, 10f, groundMask, QueryTriggerInteraction.Collide))
            crockSpot.position = hit.point;
    }

    public virtual void Engage()
    {
        canEngage = false;
        PlayerManager.Instance.canMove = false;
        PlayerManager.Instance.player.GetComponent<PlayerMove>().SetVelocity(Vector3.zero);

        anim.SetBool("Talking", true);
        originalCam = CameraManager.Instance.currentCamera;
        CameraManager.Instance.SetCamera(dialogue.cameras[0], 1.8f);

        PlayerManager.Instance.player.GetComponent<PlayerCutsceneMove>().Move(crockSpot);

        //initiate dialogue
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public virtual void Disengage()
    {
        anim.SetBool("Talking", false);
        CameraManager.Instance.SetCamera(originalCam.gameObject, 1.5f);

        StartCoroutine("Reenable");
    }

    public virtual void ToggleTalking(bool speak)
    {
        anim.SetBool("Talking", speak);
    }

    IEnumerator Reenable()
    {
        yield return new WaitForSeconds(0.7f);
        canEngage = true;
        PlayerManager.Instance.canMove = true;
    }
}
