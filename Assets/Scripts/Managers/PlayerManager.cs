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

    AllTransitionObjects allTransitionObjects;
    public int transitionFlag;

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
        stone,
        ghost
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
        //canMove = true;

        currentHitSound = hitSounds[0];

        //puts crock at the correct transition destination as determined by the exit he left in the last scene
        allTransitionObjects = GameObject.Find("AllTransitionObjects").GetComponent<AllTransitionObjects>();
        if (!allTransitionObjects.DebugMode)
        {
            Transform destination = allTransitionObjects.allTransitions[transitionFlag].destPoint;
            Transform camPos = allTransitionObjects.allTransitions[transitionFlag].camPoint;
            player.transform.position = destination.position;
            player.transform.rotation = destination.rotation;

            StartCoroutine("ForceCamera", camPos.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentState = PlayerState.normal;
        currentForm = PlayerForm.none;
        //canMove = true;

        transitionFlag = 0;

        currentHitSound = hitSounds[0];
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //TAKE THIS OUT AFTER TESTING
        if (Input.GetKey(KeyCode.LeftBracket))
        {
            Time.timeScale = 6f;
        } else if (Input.GetKey(KeyCode.RightBracket))
        {
            Time.timeScale = 1f;
        }
        */
    }

    //this is used to wait a frame before forcing the camera position.
    //for some reason, if the camera is forced on the first frame of the scene, it makes the m_yaxis variable 0 and forces the camera into the ground
    IEnumerator ForceCamera(GameObject camPoint)
    {
        yield return null;
        CameraManager.Instance.currentCamera.ForceCameraPosition(camPoint.transform.position, camPoint.transform.rotation);
    }
}
