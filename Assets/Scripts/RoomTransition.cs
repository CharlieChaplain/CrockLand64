using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    public Transform destination;
    public GameObject newCam;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9) //9 = player
        {
            StartCoroutine("transition");
        }
    }

    IEnumerator transition()
    {
        Pause.activatePause = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        PlayerManager.Instance.canMove = false;
        IrisWipe.Instance.WipeOut();

        float waitTime = 4f;

        yield return new WaitForSeconds(waitTime / 2f);

        player.GetComponent<PlayerMove>().SetVelocity(Vector3.zero);

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = destination.position;
        player.transform.forward = destination.forward;
        player.GetComponent<CharacterController>().enabled = true;

        if (newCam != null)
        {
            CameraManager.Instance.SetCamera(newCam, 0);
        }

        yield return new WaitForSeconds(waitTime / 2f);

        IrisWipe.Instance.WipeIn();
        PlayerManager.Instance.canMove = true;

        Pause.activatePause = true;
    }
}
