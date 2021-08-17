using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest_Logic : Enemy 
{
    bool opened = false;

    public List<GameObject> wealth;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 && other.CompareTag("HurtBox")) //9 = player
        {
            PlaySound hurtSound = null;
            hurtSound = PlayerManager.Instance.currentHitSound;
            Hurt(other.GetComponent<HurtBoxInfo>().damage, other.transform.position, hurtSound);

            if (health <= 0 && !opened)
            {
                Open();
            }
        }
    }

    void Open()
    {
        opened = true;
        anim.SetTrigger("Unlock");
        StartCoroutine("SpawnWealth");

        GetComponent<CharacterController>().enabled = false;
    }

    IEnumerator SpawnWealth()
    {
        int count = wealth.Count;
        for(int i = 0; i < count; i++)
        {
            Vector3 spawnDir = Quaternion.Euler(0, (360f / count) * i, 0) * Vector3.forward;
            GameObject coin = GameObject.Instantiate(wealth[i], transform.position + Vector3.up, Quaternion.identity);

            spawnDir.y = 4f;
            spawnDir.Normalize();

            coin.GetComponent<Rigidbody>().AddForce(spawnDir * 350f);
            coin.GetComponent<Wealth>().gravityAffected = true;

            yield return new WaitForSeconds(.2f);
        }
    }
}
