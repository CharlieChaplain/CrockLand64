using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleDayNight : MonoBehaviour
{
    public GameObject heavenlyBodies; //the parent game object of the sun and moon. Gets rotated
    public float speed; //should match with the skybox shader
    public float offset; //should match with the skybox shader

    public Light dayLight;
    public Light nightLight;

    float dayLightIntensity;
    float nightLightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        dayLightIntensity = dayLight.intensity;
        nightLightIntensity = nightLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        float lerpValue = (Time.time + offset) * speed;
        heavenlyBodies.transform.rotation = Quaternion.Euler(0, 0, -lerpValue * Mathf.Rad2Deg);

        dayLight.intensity = dayLightIntensity * ((Mathf.Sin(lerpValue) + 1f) / 2f);
        nightLight.intensity = nightLightIntensity * (1f - ((Mathf.Sin(lerpValue) + 1f) / 2f));
    }
}
