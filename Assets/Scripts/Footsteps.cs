using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public List<PlaySound> footstepSounds;

    public List<PlaySound> waterFootsteps;
    public GameObject particlesPrefab;
    public Collider waterFootstepTriggerCollider;

    public bool isInWater; //this will be provided to this script by another script on the same game object (In crock's case, it's Swim)
    public float halfWidth; //half the distance between feet, will be multiplied by right/left vector and added to transform to calc location of sound
    public float halfGait; //the distance between transform.pos and the point on the forward axis the foot falls

    public LayerMask waterMask;

    public virtual void PlayFootstep(int right) //determines if right step (1) or left step (-1)
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
        } else
        {
            int rand = Random.Range(0, footstepSounds.Count - 1);
            footstepSounds[rand].Play(spawnLoc);
        }
    }
}
