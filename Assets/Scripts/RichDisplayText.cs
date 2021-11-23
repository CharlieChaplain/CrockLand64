using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RichDisplayText : MonoBehaviour
{
    public Vector2 textboxSize;
    Vector2 position; //the current position of the cursor at the top left of the characterbox

    Color textColor;

    Color baseColor = Color.white;
    Color highlightColor = new Color(235f, 150f, 2f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Type()
    {

    }
}

public class RDTChar : MonoBehaviour
{
    Vector3 initPos;
    public void SetValues(bool _shaking)
    {
        initPos = transform.position;
        if (_shaking)
        {
            StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        transform.position = initPos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        yield return null;
    }
}
