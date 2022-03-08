using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransition : MonoBehaviour
{
    public enum TransitionType
    {
        inLevel,
        otherLevel
    };
    public TransitionType type;

    public Transform walkPoint;
    public Transform camPoint; //leave campoint null if the camera you are switching to isn't a freelook cam

    [Header("If in level transition")]
    public RoomTransition destination;
    public Transform destPoint;
    public GameObject newCam;

    [Header("If other level transition")]
    public string levelToLoad;
    public int entryFlag; //if a map has multiple entrances, then it will have multiple spawns. This flag determines which spawn to use in the next scene

    private bool transitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9 && !transitioning) //9 = player
        {
            if(type == TransitionType.inLevel)
            {
                transitioning = true;
                StartCoroutine("InterTransition");
            }else if (type == TransitionType.otherLevel)
            {
                transitioning = true;
                StartCoroutine("IntraTransition");
            }
        }
    }

    IEnumerator InterTransition()
    {
        Pause.activatePause = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        PlayerManager.Instance.canMove = false;
        player.GetComponent<PlayerCutsceneMove>().Move(walkPoint);
        if (camPoint != null)
            CameraManager.Instance.SetCamera(camPoint.gameObject, 1f);
        IrisWipe.Instance.WipeOut();

        float waitTime = 4f;

        yield return new WaitForSeconds(waitTime / 2f);

        player.GetComponent<PlayerMove>().SetVelocity(Vector3.zero);
        player.GetComponent<PlayerCutsceneMove>().StopMove();
        player.GetComponent<CharacterController>().enabled = false;
        //drops crock at the dest point
        player.transform.position = destination.destPoint.position;
        player.transform.forward = destination.destPoint.forward;

        if (newCam != null)
        {
            CameraManager.Instance.SetCamera(newCam, 0);
        }
        if(destination.camPoint != null)
            CameraManager.Instance.currentCamera.ForceCameraPosition(destination.camPoint.position,
                destination.camPoint.rotation);

        yield return new WaitForSeconds(waitTime / 2f);
        player.GetComponent<CharacterController>().enabled = true;

        IrisWipe.Instance.WipeIn();
        PlayerManager.Instance.canMove = true;

        Pause.activatePause = true;
        transitioning = false;
    }

    IEnumerator IntraTransition()
    {
        Pause.activatePause = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        PlayerManager.Instance.canMove = false;
        player.GetComponent<PlayerCutsceneMove>().Move(walkPoint);
        if (camPoint != null)
            CameraManager.Instance.SetCamera(camPoint.gameObject, 1f);
        IrisWipe.Instance.WipeOut();

        float waitTime = 4f;

        yield return new WaitForSeconds(waitTime / 2f);

        PlayerManager.Instance.transitionFlag = entryFlag;

        SceneManager.LoadScene(levelToLoad);
    }
}
