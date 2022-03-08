using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    public Animator mainAnim;
    public Animator titleAnim; //the game title and "Press Start"
    public Animator copyMessageAnim; //appears when player is selecting which file to copy overtop of
    public MainMenuCursor mmCursor;

    public Image background; //the background behind the file select options
    public Image lerpImage; //used to lerp between colors when changing them
    public List<Sprite> backgrounds; //the colors to flip between

    //throbbers activate when mode is active and indicate that the mode is active
    public GameObject copyThrobber;
    public GameObject eraseThrobber;
    public List<GameObject> fileThrobbers;
    Vector2 throbberInitSize;

    public bool canReturn; //determines if pressing the "cancel" button takes player back to "Press Start" screen

    public VerifyMenu vMenu;

    public List<TitleFileImages> fileImages;

    bool copyingFile = false;
    int copyWhichFile = -1;

    enum Mode
    {
        normal,
        copy,
        erase
    };
    Mode mode = Mode.normal;

    // Start is called before the first frame update
    protected override void Start()
    {
        RecolorOptions();
        mmCursor.ResetMenu();
        mainAnim = GetComponent<Animator>();

        throbberInitSize = copyThrobber.GetComponent<RectTransform>().sizeDelta;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //makes sure we are "paused" and on the correct menu (this one)
        if (!PlayerManager.Instance.paused || !pause.CheckMenu(0) || !active)
            return;
        //handles moving the cursor
        if (!pressed)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                pressed = true;
                mmCursor.MoveUp();
                RecolorOptions();

                CheckTrembling();
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                pressed = true;
                mmCursor.MoveDown();
                RecolorOptions();

                CheckTrembling();
            }

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                pressed = true;
                mmCursor.MoveRight();
                RecolorOptions();
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                pressed = true;
                mmCursor.MoveLeft();
                RecolorOptions();
            }
        }
        //will prevent the stick from being held to scroll every frame
        if (Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
            pressed = false;

        canReturn = mode == Mode.normal;
    }

    public override void Logic()
    {
        if (!active)
            return;
        if (Input.GetButtonDown("Submit"))
        {
            switch (mmCursor.GetIndex())
            {
                case 0:
                    //file 1
                    Action(1);
                    break;
                case 1:
                    //file 2
                    Action(2);
                    break;
                case 2:
                    //file 3
                    Action(3);
                    break;
                case 3:
                    //copy
                    if (mode != Mode.copy)
                    {
                        mode = Mode.copy;
                        ChangeBGColor(1);
                        RecolorOptions();
                        StopCoroutine("Throb");
                        eraseThrobber.GetComponent<Image>().rectTransform.sizeDelta = throbberInitSize;
                        eraseThrobber.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0);
                        StartCoroutine("Throb", copyThrobber);

                        foreach(TitleFileImages fImages in fileImages)
                            fImages.StopTremble();
                    }
                    else
                    {
                        mode = Mode.normal;
                        ChangeBGColor(0);
                        RecolorOptions();
                        StopCoroutine("Throb");
                        copyThrobber.GetComponent<Image>().rectTransform.sizeDelta = throbberInitSize;
                        copyThrobber.GetComponent<Image>().color = new Color(1f,1f,1f,0);

                        DisengageCopying();
                    }
                    break;
                case 4:
                    //erase
                    if (mode != Mode.erase)
                    {
                        mode = Mode.erase;
                        ChangeBGColor(2);
                        RecolorOptions();
                        StopCoroutine("Throb");
                        copyThrobber.GetComponent<Image>().rectTransform.sizeDelta = throbberInitSize;
                        copyThrobber.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0);
                        StartCoroutine("Throb", eraseThrobber);

                        DisengageCopying();
                    }
                    else
                    {
                        mode = Mode.normal;
                        ChangeBGColor(0);
                        RecolorOptions();
                        StopCoroutine("Throb");
                        eraseThrobber.GetComponent<Image>().rectTransform.sizeDelta = throbberInitSize;
                        eraseThrobber.GetComponent<Image>().color = new Color(1f,1f,1f,0);

                        foreach (TitleFileImages fImages in fileImages)
                            fImages.StopTremble();
                    }
                    break;
                case 5:
                    //options
                    pause.ChangeMenu(1);
                    break;
                case 6:
                    //quit
                    Debug.Log("quit");
                    break;
                default:
                    break;
            }
        }

        if (Input.GetButtonDown("Cancel"))
        {
            if (mode == Mode.copy || mode == Mode.erase)
            {
                mode = Mode.normal;
                ChangeBGColor(0);
                RecolorOptions();
                StopCoroutine("Throb");
                copyThrobber.GetComponent<Image>().rectTransform.sizeDelta = throbberInitSize;
                eraseThrobber.GetComponent<Image>().rectTransform.sizeDelta = throbberInitSize;
                copyThrobber.GetComponent<Image>().color = new Color(1f,1f,1f,0);
                eraseThrobber.GetComponent<Image>().color = new Color(1f,1f,1f,0);

                foreach (TitleFileImages fImages in fileImages)
                    fImages.StopTremble();

                DisengageCopying();
            }
        }
    }

    void EngageCopying(int file)
    {
        copyingFile = true;
        copyWhichFile = file;
        copyMessageAnim.SetBool("Visible", true);
        StartCoroutine("Glow", fileThrobbers[file - 1]);
    }
    void DisengageCopying()
    {
        copyMessageAnim.SetBool("Visible", false);
        copyingFile = false;

        StopCoroutine("Glow");
        foreach (GameObject image in fileThrobbers)
            image.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0);
    }
    void CopyLogic()
    {
        if (!copyingFile || mode != Mode.copy)
            return;

        if (Input.GetButtonDown("Cancel"))
        {
            mode = Mode.normal;
            ChangeBGColor(0);
            RecolorOptions();
            StopCoroutine("Throb");
            copyThrobber.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0);

            DisengageCopying();
        }
    }

    //changes the background color of each file to show copy/erase mode
    void ChangeBGColor(int index)
    {
        StartCoroutine("LerpImages", index);
    }

    protected override void RecolorOptions()
    {
        foreach (Image image in optionImages)
        {
            image.color = new Color(0.5849056f, 0.5849056f, 0.5849056f);
        }
        optionImages[mmCursor.GetIndex()].color = Color.white;
        if (mode == Mode.copy)
            optionImages[3].color = Color.white;
        if (mode == Mode.erase)
            optionImages[4].color = Color.white;
    }

    public override void Enter()
    {
        base.Enter();
        mainAnim.SetBool("Show", true);
        titleAnim.SetBool("Show", false);
    }

    public override void Leave()
    {
        mainAnim.SetBool("Show", false);

        if(!PlayerManager.Instance.paused)
            titleAnim.SetBool("Show", true);
    }

    public override void AcceptVerification()
    {
        Debug.Log("yes");
    }

    public override void SetActive(bool _active)
    {
        base.SetActive(_active);
        //sets mode to normal if becoming active
        if (_active)
        {
            mode = Mode.normal;
            ChangeBGColor(0);
            RecolorOptions();
            StopCoroutine("Throb");
            copyThrobber.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0);
            eraseThrobber.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0);

            foreach (TitleFileImages fImages in fileImages)
                fImages.StopTremble();

            DisengageCopying();
        }
    }

    //determines what to do when a file is selected
    void Action(int fileNum)
    {
        switch (mode){
            case Mode.copy:
                if (!copyingFile)
                    EngageCopying(fileNum);
                else
                    vMenu.Enter(1);
                break;
            case Mode.erase:
                vMenu.Enter(2);
                break;
            default:
                vMenu.Enter(0);
                break;
        }
    }

    //use these next 4 functions to finalize each action.
    void NewGame()
    {
        Debug.Log("newgame");
    }

    void ContinueGame()
    {
        Debug.Log("continuegame");
    }

    void Copy(int fileToCopy, int fileOverwritten)
    {
        Debug.Log("Copy " + fileToCopy + " to " + fileOverwritten);
    }

    void Erase()
    {
        Debug.Log("Erase");
    }

    //will check to see if images should tremble in fear of being erased, if the cursor is hovering them and the mode is erase
    void CheckTrembling()
    {
        if (mode == Mode.erase && mmCursor.GetIndex() <= 2)
        {
            for (int i = 0; i < fileImages.Count; i++)
            {
                if (i == mmCursor.GetIndex())
                    fileImages[i].StartTremble();
                else
                    fileImages[i].StopTremble();
            }
        }
        else if(mmCursor.GetIndex() > 2)
        {
            foreach (TitleFileImages fImages in fileImages)
                fImages.StopTremble();
        }
    }

    IEnumerator LerpImages(int index)
    {
        float timeToLerp = 0.3f;
        lerpImage.sprite = background.sprite;
        lerpImage.color = Color.white;
        background.sprite = backgrounds[index];

        for(float f = 0; f < timeToLerp; f += Time.deltaTime)
        {
            float alpha = 1f - (f / timeToLerp);
            lerpImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        lerpImage.color = new Color(1f, 1f, 1f, 0);
    }

    IEnumerator Throb(GameObject image)
    {
        Vector2 initSize = image.GetComponent<RectTransform>().sizeDelta;
        float speed = 0.7f;

        while (true)
        {
            float timer = 0;
            while (timer < speed)
            {
                image.GetComponent<RectTransform>().sizeDelta = initSize * Mathf.Lerp(1f, 1.3f, timer / speed);
                image.GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0, timer / speed));
                timer += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
    }

    IEnumerator Glow(GameObject image)
    {
        float speed = 6f;
        while (true)
        {
            /*
            float timer = 0;
            while (timer < speed)
            {
                image.GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0, timer / speed));
                timer += Time.deltaTime;
                yield return null;
            }
            */
            image.GetComponent<Image>().color = new Color(1f, 1f, 1f, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
            yield return null;
        }
    }
}
