using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMenu : Menu
{
    public Animator soundMenuAnim;

    private bool alteredSound;

    public VolumeSlider musicSlider;
    public VolumeSlider sfxSlider;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        alteredSound = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Logic()
    {
        if (Input.GetAxisRaw("Horizontal") > 0 && !alteredSound)
        {
            alteredSound = true;
            if (cursor.currentOption == 0)
            {
                SoundManager.Instance.musicVolume = musicSlider.VolumeUp();
            }
            else if (cursor.currentOption == 1)
            {
                SoundManager.Instance.soundEffectVolume = sfxSlider.VolumeUp();
            }
        }
        else if (Input.GetAxisRaw("Horizontal") < 0 && !alteredSound)
        {
            alteredSound = true;
            if (cursor.currentOption == 0)
            {
                SoundManager.Instance.musicVolume = musicSlider.VolumeDown();
            }
            else if (cursor.currentOption == 1)
            {
                SoundManager.Instance.soundEffectVolume = sfxSlider.VolumeDown();
            }
        }
        else if (Input.GetAxisRaw("Horizontal") == 0)
            alteredSound = false;

        if (Input.GetButtonDown("Submit"))
        {
            switch (cursor.currentOption)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    pause.ChangeMenu(1);
                    break;
                default:
                    break;
            }
        }

        if (Input.GetButtonDown("Cancel"))
            pause.ChangeMenu(1);
    }

    public override void Enter()
    {
        base.Enter();
        soundMenuAnim.SetBool("Show", true);
    }
    public override void Leave()
    {
        base.Leave();
        soundMenuAnim.SetBool("Show", false);
    }
}
