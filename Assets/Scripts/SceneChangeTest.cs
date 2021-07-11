using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Punch")> 0)
        {
            SceneDirector.LoadScene("TutorialTidal");
        }
    }
}
