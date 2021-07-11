using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seagull : MonoBehaviour
{
    private float actionTimer;
    private Animator anim;
    private CharacterController controller;

    public bool flying = false;
    private bool leaving = false;
    public Vector3 flyDirection;

    public float flySpeed = 10;

    public Camera cam;
    public float tolerance = 20f;

    public LayerMask groundMask;
    
    public GameObject home; //the spawner that this seagull is attached to

    // Start is called before the first frame update
    void Start()
    {
        actionTimer = Random.Range(2f, 4f);
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!flying)
        {
            actionTimer -= Time.deltaTime;

            if (actionTimer <= 0)
            {
                if (Random.value < 0.5f)
                    Peck();
                else
                    Hop();

                actionTimer = Random.Range(2f, 4f);
            }
        } else
        {
            Fly();
        }

        anim.SetBool("Flying", flying);
    }

    void Hop()
    {
        anim.SetTrigger("Hop");
        StartCoroutine("HopTwist");
    }

    void Peck()
    {
        anim.SetTrigger("Peck");
    }

    public void FlyAway(Vector3 suggestedDirection)
    {
        flying = true;
        leaving = true;

        //flyDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f));
        flyDirection = Quaternion.Euler(new Vector3(0, Random.Range(-30f, 30f), 0)) * suggestedDirection;
        flyDirection.y = Random.Range(0.5f, 1f);
        flyDirection.Normalize();

        transform.forward = flyDirection;
    }

    void Fly()
    {
        controller.Move(flyDirection * flySpeed * Time.deltaTime);

        if (leaving && !IsInView())
        {
            Destroy(this.gameObject);
        }

        if (!leaving &&
            Physics.CheckSphere(transform.position + controller.center, controller.radius, groundMask) &&
            Vector3.Distance(transform.position, home.transform.position) <= home.GetComponent<SeagullSpawner>().GetRadius())
        {
            flying = false;
            Vector3 newForward = transform.forward;
            newForward.y = 0;
            transform.forward = newForward;
        }
    }

    bool IsInView()
    {
        Vector3 pos = cam.WorldToScreenPoint(transform.position);
        if ((pos.x >= 0 - tolerance && pos.x <= Screen.width + tolerance) && (pos.y >= 0 - tolerance && pos.y <= Screen.height + tolerance))
        {
            return true;
        }
        return false;

    }

    IEnumerator HopTwist()
    {
        float length = .16f;
        float timer = length;
        float rotSpeed = Random.Range(30f,120f);
        if (Random.value < 0.5f)
            rotSpeed *= -1f;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            transform.Rotate(Vector3.up * (rotSpeed * Time.deltaTime / length));
            yield return null;
        }


    }
}
