using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Image[] sliderPips;

    int currentVolume;

    const float INCREMENT = .0625f; //1/16, because there are 16 pips in the slider

    public PlaySound menuPipSound;

    // Start is called before the first frame update
    void Start()
    {
        currentVolume = sliderPips.Length;
    }

    public float VolumeUp()
    {
        if (currentVolume >= sliderPips.Length - 1)
        {
            sliderPips[sliderPips.Length - 1].color = Color.white;
            currentVolume = 16;

            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);

            return 1f;
        }

        sliderPips[currentVolume].color = Color.white;
        currentVolume++;

        menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);

        return currentVolume * INCREMENT;
    }
    public float VolumeDown()
    {
        if (currentVolume == 0)
            return 0;

        sliderPips[currentVolume - 1].color = Color.gray;
        currentVolume--;

        menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);

        return currentVolume * INCREMENT;
    }
}
