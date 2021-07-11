using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsExt : Footsteps
{
    public List<PlaySound> dirtFootsteps;
    public List<PlaySound> woodFootsteps;
    public List<PlaySound> sandFootsteps;
    public List<PlaySound> stoneFootsteps;

    public LayerMask groundMask;

    public override void PlayFootstep(int right)
    {
        Vector3 spawnLoc = transform.position + (transform.right * right * halfWidth) + (transform.forward * halfGait);

        if (isInWater)
        {
            int rand = Random.Range(0, waterFootsteps.Count - 1);

            RaycastHit hit;

            //creates ray and then reverses the direction so that it'll see the top of the water from the outside
            Ray splooshRay = new Ray(transform.position, Vector3.up);
            splooshRay.origin = splooshRay.GetPoint(5f);
            splooshRay.direction *= -1f;

            if (Physics.Raycast(splooshRay, out hit, 5f, waterMask, QueryTriggerInteraction.Collide))
            {
                spawnLoc.y = hit.transform.position.y + (hit.transform.localScale.y / 2f);
            }

            waterFootsteps[rand].Play(spawnLoc);

            GameObject partPrefab = Instantiate(particlesPrefab, spawnLoc, Quaternion.identity);
            partPrefab.GetComponent<waterFootstepInit>().Init(waterFootstepTriggerCollider);

            Destroy(partPrefab, 5f);
        }
        else
        {
            RaycastHit hit;

            //checks for ground directly below character
            if (Physics.Raycast(transform.position + (Vector3.up * 1.0f), Vector3.down, out hit, 5f, groundMask, QueryTriggerInteraction.Collide))
            {
                //checks if ground has ground info component (failsafe)
                GroundInfo info = hit.transform.GetComponent<GroundInfo>();
                if (info != null)
                {
                    //determines which sound list to play
                    switch (info.groundType)
                    {
                        case GroundInfo.GroundTypes.dirt:
                            PickSound(dirtFootsteps).Play(spawnLoc);
                            break;
                        case GroundInfo.GroundTypes.sand:
                            PickSound(sandFootsteps).Play(spawnLoc);
                            break;
                        case GroundInfo.GroundTypes.stone:
                            PickSound(stoneFootsteps).Play(spawnLoc);
                            break;
                        case GroundInfo.GroundTypes.wood:
                            PickSound(woodFootsteps).Play(spawnLoc);
                            break;
                        default:
                            break;
                    }//end switch
                }
                else
                {
                    Debug.Log("no groundinfo");
                }//end component check
            }//end raycast
        }//end if/else
    }//end play footsteps

    PlaySound PickSound(List<PlaySound> list)
    {
        int rand = Random.Range(0, list.Count - 1);
        return list[rand];
    }
}
