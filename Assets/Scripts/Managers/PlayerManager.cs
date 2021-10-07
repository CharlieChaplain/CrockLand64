using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public GameObject player;

    public Vector3 faceDir; //the direction the player is facing (not looking via camera)

    //public int wealth;
    //public int displayWealth;

    public PlaySound currentHitSound; //the current sound to be played when crock hits something

    public bool paused;

    public enum PlayerState
    {
        normal,
        charging,
        crouching,
        sliding,
        swimming,
        carrying,
        ladder,
        hurt,
        transformed
    }

    public enum PlayerForm
    {
        none,
        stone
    }

    public PlayerState currentState;
    public PlayerForm currentForm;
    public bool canMove;

    public List<PlaySound> hitSounds;

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

    public void OnSceneLoad()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentState = PlayerState.normal;
        currentForm = PlayerForm.none;
        canMove = true;

        currentHitSound = hitSounds[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentState = PlayerState.normal;
        currentForm = PlayerForm.none;
        canMove = true;

        currentHitSound = hitSounds[0];
    }

    // Update is called once per frame
    void Update()
    {
        //TAKE THIS OUT AFTER TESTING
        if (Input.GetKey(KeyCode.LeftBracket))
        {
            Time.timeScale = 6f;
        } else if (Input.GetKey(KeyCode.RightBracket))
        {
            Time.timeScale = 1f;
        }
    }
    /*
    public void AddWealth(int amount)
    {
        wealth += amount;
    }
    */
}
