using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    GameObject nextArrow;
    Image portrait;

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
        nextArrow = GameObject.Find("NextArrow");
        nextArrow.SetActive(false);

        portrait = GameObject.Find("Portrait").GetComponent<Image>();
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

        portrait.sprite = currentDialogue.portraits[0];

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

        string sentence = sentences.Dequeue();

        //camera change::   _CAM-0-0    where first number is camera number in Dialogue camera array and second is lerp time in milliseconds
        //portrait change::   _POR-0    where the number is portrait number in Dialogue portrait array
        if (sentence.Substring(0, 4) == "_CAM")
        {
            string[] parameters = sentence.Split('-');
            //makes sure values are parsable
            int index = int.Parse(parameters[1]);
            if (index >= parameters.Length)
                Debug.Log("Camera change index out of bounds!");
            else
                CameraManager.Instance.SetCamera(currentDialogue.cameras[index], int.Parse(parameters[2]) / 1000f);

            NextSentence();
        }
        else if(sentence.Substring(0, 4) == "_POR")
        {
            string[] parameters = sentence.Split('-');
            //makes sure values are parsable
            int index = int.Parse(parameters[1]);
            if(index >= parameters.Length)
                Debug.Log("Portrait change index out of bounds!");
            else
                portrait.sprite = currentDialogue.portraits[index];

            NextSentence();
        }
        else
        {
            nextArrow.SetActive(false);
            currentDialogue.npc.ToggleTalking(true);
            
            StartCoroutine("Type", sentence);
        }
    }

    void EndDialogue()
    {
        dialogueAnim.SetBool("Visible", false);
        talking = false;
        nextArrow.SetActive(false);
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
        nextArrow.SetActive(true);
    }
}
