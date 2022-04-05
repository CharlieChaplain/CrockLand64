using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //controls = GetComponent<_InputControl>().GetControl();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool keyboardInput = true;
    public _Controls controls;

    public void OnSceneLoad()
    {
        controls = GetComponent<_InputControl>().GetControl();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
