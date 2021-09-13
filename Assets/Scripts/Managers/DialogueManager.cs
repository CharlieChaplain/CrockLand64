using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Queue<string> sentences;
    bool displayFinished; //determines if the display is finished writing all the letters.
    bool talking;

    //UI variables
    GameObject dialogueBox;
    Animator dialogueAnim;
    TextMeshProUGUI dialogueText;

    //changes per dialogue
    Dialogue currentDialogue;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        talking = false;
        displayFinished = false;

        dialogueBox = GameObject.Find("Dialogue");

        dialogueAnim = dialogueBox.GetComponent<Animator>();
        dialogueText = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (talking)
        {
            if (Input.GetButtonDown("Punch") && displayFinished)
            {
                NextSentence();
                displayFinished = false;
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;

        sentences.Clear();

        dialogueAnim.SetBool("Visible", true);

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        talking = true;
        NextSentence();
    }
    public void NextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        //DO COMMAND LOGIC HERE IN THIS IF/ELSE
        if (false)
        {

        }else
        {
            currentDialogue.npc.ToggleTalking(true);
            string sentence = sentences.Dequeue();
            StartCoroutine("Type", sentence);
        }

    }

    void EndDialogue()
    {
        dialogueAnim.SetBool("Visible", false);
        talking = false;
        currentDialogue.npc.Disengage();
    }

    //this function is used to play gobbledygook noises as the character talks. These noises are stored in the Dialogue class in an array
    void GobbledyGook()
    {

    }

    //types out the messages character by character
    IEnumerator Type(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;

            //will speed up the typing if jump is held
            if (Input.GetButton("Jump"))
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(0.04f);
            }
        }

        displayFinished = true;
        currentDialogue.npc.ToggleTalking(false);
    }
}
