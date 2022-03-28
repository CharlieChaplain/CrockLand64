using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphinxBlink : MonoBehaviour
{
    public List<Texture2D> allFaces;
    public Material faceMat;
    public GameObject spotlight;
    public PlaySound meowSO;
    GameObject spotlightMesh;
    Vector3 initSpotlightSize;
    Quaternion initSpotlightRot;

    bool on = false;

    // Start is called before the first frame update
    void Start()
    {
        faceMat.mainTexture = allFaces[0];
        spotlightMesh = spotlight.transform.GetChild(0).gameObject;
        initSpotlightSize = spotlightMesh.transform.localScale;
        initSpotlightRot = spotlight.transform.rotation;
    }

    private void Update()
    {
        if(!on && PlayerManager.Instance.currentForm == PlayerManager.PlayerForm.ghost)
        {
            on = true;
            Open();
        }

        if(on && PlayerManager.Instance.currentForm != PlayerManager.PlayerForm.ghost)
        {
            on = false;
            Close();
        }
    }

    private void Open()
    {
        StartCoroutine("EyesOpen");
    }

    private void Close()
    {
        StartCoroutine("EyesClose");
    }

    IEnumerator EyesOpen()
    {
        for (int currentFrame = 0; currentFrame < allFaces.Count; currentFrame++)
        {
            faceMat.mainTexture = allFaces[currentFrame];
            yield return new WaitForEndOfFrame();
        }

        //play sound and activate beam here
        meowSO.Play(transform.position + new Vector3(0, 15.9f, 17.2f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("Shine");
        yield return new WaitForSeconds(1f);
        StartCoroutine("Rotate");
    }
    IEnumerator EyesClose()
    {
        for (int currentFrame = allFaces.Count - 1; currentFrame >= 0; currentFrame--)
        {
            faceMat.mainTexture = allFaces[currentFrame];
            yield return new WaitForEndOfFrame();
        }

        spotlight.SetActive(false);
        StopCoroutine("Rotate");
        spotlight.transform.rotation = initSpotlightRot;
    }

    IEnumerator Shine()
    {
        spotlightMesh.transform.localScale = new Vector3(spotlightMesh.transform.localScale.x, 0, spotlightMesh.transform.localScale.z);
        spotlight.SetActive(true);
        float time = 0.3f;
        for (float f = 0; f < time; f += Time.deltaTime)
        {
            spotlightMesh.transform.localScale = new Vector3(spotlightMesh.transform.localScale.x,
                initSpotlightSize.y * (f / time),
                spotlightMesh.transform.localScale.z);
            yield return null;
        }

        spotlightMesh.transform.localScale = initSpotlightSize;
    }

    IEnumerator Rotate()
    {
        float offset = Time.time;
        while (true)
        {
            float angle = Mathf.Sin((Time.time - offset) * 0.7f) * 20f;
            spotlight.transform.rotation = initSpotlightRot * Quaternion.Euler(0, 0, angle);
            yield return null;
        }
    }
}
