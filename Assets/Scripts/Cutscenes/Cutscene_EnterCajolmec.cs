using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class Cutscene_EnterCajolmec : Cutscene
{
    GameObject player;
    public GameObject cutsceneCrock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //advances to next cut after dialogue is done
        if(DialogueManager.Instance.DisplayFinished() && Input.GetButton("Punch"))
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9) //9 == player
        {
            Debug.Log("startcutscene");
            player = other.gameObject;
            PlayCutscene();
        }
    }

    public override void PlayCutscene()
    {
        //remove player from the level for the cutscene
        PlayerManager.Instance.canMove = false;
        player.GetComponent<PlayerMove>().SetSpeed(0);
        player.GetComponent<PlayerMove>().SetVelocity(Vector3.zero);
        player.GetComponent<Attack>().StopAttack();
        PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
        player.SetActive(false);

        //add in cutscene crock
        cutsceneCrock.SetActive(true);

        GetComponent<PlayableDirector>().Play();
    }
}
