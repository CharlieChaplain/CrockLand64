using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurt : MonoBehaviour
{
    bool hurting;
    bool invincible;
    bool visible = true;
    float iTimer = 3f;

    public Animator anim;
    public SkinnedMeshRenderer mesh;

    public GameObject doubloon; //will be dropped on getting hurt

    /// <summary>
    /// Hurts crock.
    /// </summary>
    /// <param name="power">how far crock is thrown back</param>
    /// <param name="intensity">how many coins crock loses</param>
    /// <param name="variance">how many coins randomly up or down from "intensity" he will lose</param>
    /// <param name="source">the source location of the pain. Crock will fly back from this location</param>
    public void HurtPlayer(float power, int intensity, int variance, Vector3 source)
    {
        //will only hurt crock if he is not still invincible from getting hurt
        if (invincible)
            return;

        GetComponent<Charge>().StopCharge(); //THIS ALSO SETS PLAYER STATE TO NORMAL, could cause some cheeky errors in the future
        GetComponent<Attack>().StopAttack();

        invincible = true;
        PlayerManager.Instance.canMove = false;
        iTimer = 3f;

        anim.SetBool("Hurting", true);

        Vector3 direction = transform.position - source;
        direction.y = 0;
        direction.Normalize();

        transform.forward = -direction;

        direction.y = 2f;
        direction.Normalize();
        GetComponent<PlayerMove>().SetVelocity(direction * power);
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
        yield return new WaitForSeconds(1f);
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
