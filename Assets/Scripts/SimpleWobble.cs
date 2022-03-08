using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWobble : MonoBehaviour
{
    public float time;
    public float amplitude;
    public bool[] wobbleAxes = new bool[3];

    float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(time + 1f, time - 1f);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            timer = Random.Range(time + 2f, time - 2f);
            StartCoroutine("Wobble");
        }
    }

    IEnumerator Wobble()
    {
        for (float i = 0; i < 2; i += 0.2f)
        {
            float wobble = (Mathf.Sin(i * Mathf.PI) - 1f) * amplitude;
            Quaternion rot = Quaternion.identity;
            if (wobbleAxes[0])
                rot *= Quaternion.Euler(wobble, 0, 0);
            if (wobbleAxes[1])
                rot *= Quaternion.Euler(0, wobble, 0);
            if (wobbleAxes[2])
                rot *= Quaternion.Euler(0, 0, wobble);
            transform.localRotation = rot;

            yield return null;
        }

        transform.GetComponent<RectTransform>().localRotation = Quaternion.identity;
    }
}
