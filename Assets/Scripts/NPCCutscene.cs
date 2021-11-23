using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
/// <summary>
/// Use this NPC script when the talking must be done in a cutscene/timeline
/// </summary>
public class NPCCutscene : NPC
{
    public Dialogue[] dialogues;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Engage(int dialogueNum)
    {
        //anim.SetBool("Talking", true);

        //initiate dialogue
        DialogueManager.Instance.StartDialogue(dialogues[dialogueNum]);
    }

    public void End()
    {
        DialogueManager.Instance.EndDialogue();
    }

    public override void Disengage()
    {
        //anim.SetBool("Talking", false);
    }

    public override void ToggleTalking(bool speak)
    {
        //anim.SetBool("Talking", speak);
    }
}
