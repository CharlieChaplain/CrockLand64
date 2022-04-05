using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    _Controls controls;

    //UI variables
    GameObject dialogueBox;
    Animator dialogueAnim;
    TextMeshProUGUI dialogueText;
    VertexJitter vertJitter;
    GameObject nextArrow;
    Image portrait;

    //changes per dialogue
    Dialogue currentDialogue;

    public void OnSceneLoad()
    {
        StartCoroutine(EnableControls());
    }
    IEnumerator EnableControls()
    {
        yield return new WaitForEndOfFrame();

        controls = InputManager.Instance.controls;

        // Input subscriptions--------------------------------------------------
        controls.EditableControls.Punch.started += PunchListener;
    }
    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        talking = false;
        displayFinished = false;

        dialogueBox = GameObject.Find("Dialogue");

        dialogueAnim = dialogueBox.GetComponent<Animator>();
        dialogueText = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        vertJitter = dialogueText.gameObject.GetComponent<VertexJitter>();
        nextArrow = GameObject.Find("NextArrow");
        nextArrow.SetActive(false);

        portrait = GameObject.Find("Portrait").GetComponent<Image>();
    }

    private void Update()
    {
    }

    void PunchListener(InputAction.CallbackContext obj)
    {
        if(talking && displayFinished)
        {
            NextSentence();
            displayFinished = false;
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
        //portrait change:: _POR-0      where the number is portrait number in Dialogue portrait array
        //jittering text::  _JIT        causes the text of that box to jitter
        //large text::      _BIG        makes fontsize twice as large
        //jitter and big::  _J&B        combines both effects of _JIT and _BIG
        //text speed::      _#.#        changes the speed of the text based on a 1 decimal precise float
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
        else if(sentence.Substring(0, 4) == "_JIT")
        {
            //changes vertJitter's parameters so it will jitter appropriately
            vertJitter.AngleMultiplier = 1f;
            vertJitter.CurveScale = 5f;
            //sets font size
            dialogueText.fontSize = 18f;

            sentence = sentence.Substring(4);
            PrepareDialogue(sentence);
        }
        else if (sentence.Substring(0, 4) == "_BIG")
        {
            //changes vertJitter's parameters so it wont jitter
            vertJitter.AngleMultiplier = 0;
            vertJitter.CurveScale = 0;
            //sets font size
            dialogueText.fontSize = 36f;

            sentence = sentence.Substring(4);
            PrepareDialogue(sentence);
        }
        else if (sentence.Substring(0, 4) == "_J&B")
        {
            //changes vertJitter's parameters so it will jitter appropriately
            vertJitter.AngleMultiplier = 1f;
            vertJitter.CurveScale = 5f;
            //sets font size
            dialogueText.fontSize = 36f;

            sentence = sentence.Substring(4);
            PrepareDialogue(sentence);
        }
        else
        {
            //changes vertJitter's parameters so it wont jitter
            vertJitter.AngleMultiplier = 0;
            vertJitter.CurveScale = 0;
            //sets font size
            dialogueText.fontSize = 18f;

            PrepareDialogue(sentence);
        }
    }

    //no matter which prefix the sentence has on it, it will go through these steps before being typed out
    void PrepareDialogue(string sentence)
    {
        float textSpeed = 1f;
        if(sentence.Substring(0, 1) == "_")
        {
            textSpeed = float.Parse(sentence.Substring(1, 3));
            sentence = sentence.Substring(4);
        }

        nextArrow.SetActive(false);
        currentDialogue.npc.ToggleTalking(true);

        //Creates variables so we can prune the sentence of special characters and use them to change the color of each char
        string newSentence = "";
        List<Color> sentenceColors = new List<Color>();

        //loops through current sentence, removes special characters, and changes the color of each accordingly.
        Color col = Color.white;
        for(int i = 0; i < sentence.Length; i++)
        {
            if(sentence[i] == '[')
            {
                col = Color.yellow;
            }else if (sentence[i] == ']')
            {
                col = Color.white;
            }
            else
            {
                sentenceColors.Add(col);
                newSentence += sentence[i];
            }
        }
        //sets the TMP text to the pruned sentence.
        dialogueText.text = newSentence;

        //resets whole textbox to be invis to start
        dialogueText.color = new Color(255f, 255f, 255f, 0);

        //Starts the typing coroutine
        dialogueText.gameObject.GetComponent<VertexColorChanger>().StartColorChanger(textSpeed, sentenceColors);
    }

    public void EndDialogue()
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

    public void FinishedTyping()
    {
        displayFinished = true;
        currentDialogue.npc.ToggleTalking(false);
        nextArrow.SetActive(true);
    }

    public bool DisplayFinished()
    {
        return displayFinished;
    }

    /* -----------OLD TYPE COROUTINE-------------  new type coroutine just loops through already present text and changes alpha from 0 to 1
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
    */
}


