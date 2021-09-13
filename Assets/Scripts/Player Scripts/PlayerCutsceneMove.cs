using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCutsceneMove : MonoBehaviour
{
    Transform destination;
    Animator anim;
    CharacterController controller;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
    }


    public void Move(Transform dest)
    {
        destination = dest;
        StartCoroutine("MovePlayer");
    }

    IEnumerator MovePlayer()
    {
        float distance;
        do
        {
            Vector3 velocity = destination.position - transform.position;
            distance = Vector3.Magnitude(velocity);
            velocity.y = 0;
            velocity.Normalize();

            transform.forward = velocity;

            controller.Move(velocity * speed * distance * Time.deltaTime);
            anim.SetFloat("Speed", speed * distance);


            yield return null;
        } while (distance > 0.2f);

        Quaternion initRot = transform.rotation;
        anim.SetFloat("Speed", 0);

        for (float f = 0; f < 1f; f += 0.1f)
        {
            transform.rotation = Quaternion.Lerp(initRot, destination.rotation, f);
            yield return null;
        }
    }
}
