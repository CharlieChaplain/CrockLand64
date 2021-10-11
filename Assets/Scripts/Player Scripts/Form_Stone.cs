using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Form_Stone : MonoBehaviour
{ 
    public float speed;
    public float turnSpeed;
    public float jumpHeight;
    public RuntimeAnimatorController animController;
    public Texture[] textures;
    public PlaySound SO_Init;
    public PlaySound SO_Inter;
    public ParticleSystem parts;

    float timer;
    int stage;

    /// <summary>
    /// call this every frame when in stone form
    /// </summary>
    public void stoneUpdate()
    {
        timer += Time.deltaTime;

        //progresses the form until after some number of seconds crock reverts to normal
        if(timer > 5f)
        {
            if(stage <= 1)
            {
                stage++;
                GetComponent<ChangeModel>().ChangeTexture(0, textures[stage]);
                timer = 0;
                parts.Emit(15);
                SO_Inter.Play(transform.position);
            } else
            {
                GetComponent<PlayerMove>().ChangeForm(PlayerManager.PlayerForm.none);
                SO_Init.Play(transform.position);
                parts.Play();
            }
        }
    }

    public void stoneInit()
    {
        timer = 0;
        stage = 0;
        GetComponent<ChangeModel>().ChangeTexture(0, textures[0]);
        SO_Init.Play(transform.position);
        parts.Play();
    }
}
