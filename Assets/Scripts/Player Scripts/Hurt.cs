using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurt : MonoBehaviour
{
    bool hurting;
    bool invincible;
    bool visible = true;
    float iTimer = 5f;

    public Animator anim;
    public SkinnedMeshRenderer mesh;

    public GameObject doubloon; //will be dropped on getting hurt

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HurtPlayer(int intensity, int variance, Vector3 source)
    {
        //will only hurt crock if he is not still invincible from getting hurt
        if (invincible)
            return;

        invincible = true;
        PlayerManager.Instance.canMove = false;
        iTimer = 5f;

        anim.SetBool("Hurting", true);

        Vector3 direction = transform.position - source;
        direction.y = 0;
        direction.Normalize();

        transform.forward = -direction;

        direction.y = 2f;
        direction.Normalize();
        GetComponent<PlayerMove>().SetVelocity(direction * 20f);
        GetComponent<PlayerMove>().SetAngleToTarget();
        GetComponent<PlayerMove>().SetSpeed(0);

        PlayerManager.Instance.currentState = PlayerManager.PlayerState.hurt;

        StartCoroutine("HurtCorout");
        StartCoroutine("Invincible");

        //drops doubloons on hurt
        int wealthLost = intensity + Random.Range(-variance, variance);

        if(wealthLost > TreasureMaster.Instance.wealth)
        {
            wealthLost = TreasureMaster.Instance.wealth;
        }

        TreasureMaster.Instance.AddWealth(-wealthLost);

        for (int i = 0; i < wealthLost; i++)
        {
            Vector3 spawnDir = Quaternion.Euler(0, (360f / wealthLost) * i, 0) * Vector3.forward;
            GameObject coin = GameObject.Instantiate(doubloon, transform.position + spawnDir + Vector3.up, Quaternion.identity);

            spawnDir.y = 3f;
            spawnDir.Normalize();

            coin.GetComponent<Rigidbody>().AddForce(spawnDir * 300f);
            coin.GetComponent<Doubloon>().gravityAffected = true;
            coin.GetComponent<Doubloon>().killTimer = 10f;
        }
    }

    public bool GetInvincible()
    {
        return invincible;
    }

    IEnumerator HurtCorout()
    {
        yield return new WaitForSeconds(2f);
        PlayerManager.Instance.canMove = true;
        anim.SetBool("Hurting", false);
        PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
    }

    IEnumerator Invincible()
    {
        while(iTimer> 0)
        {
            iTimer -= Time.deltaTime;

            visible = !visible;
            mesh.enabled = visible;

            yield return null;
        }

        mesh.enabled = true;
        invincible = false;
    }
}
