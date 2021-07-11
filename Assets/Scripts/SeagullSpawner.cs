using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullSpawner : MonoBehaviour
{
    public GameObject seagullPrefab;

    private List<GameObject> seagulls;
    
    public int maxSeagulls;
    private int currentNumSeagulls;

    private float radius;

    private float timer;

    public LayerMask groundMask;

    public bool fearless; //whether the birds fly away or not when crock is near

    public Camera cam;
    float tolerance = 20f;

    // Start is called before the first frame update
    void Start()
    {
        radius = GetComponent<SphereCollider>().radius;
        timer = 2f;

        seagulls = new List<GameObject>();

        InitPopulation();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer <= 0)
        {
            timer = 2f;

            //makes sure a seagull only spawns 1 in every 4 chances it gets
            if (currentNumSeagulls < maxSeagulls && Random.value < 0.25f)
            {
                SpawnSeagull();
            }
        }

        timer -= Time.deltaTime;
    }

    //causes a seagull to fly in from offscreen
    void SpawnSeagull()
    {
        Vector3 target = FindSpawnLocation();
        Vector3 spawnWorldPos;
        do
        {
            spawnWorldPos = target + transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(0.2f, .8f), Random.Range(-1f, 1f)).normalized * 40f;
        } while (IsInView(spawnWorldPos));

        GameObject newGull = Object.Instantiate(seagullPrefab, spawnWorldPos, Quaternion.identity);
        newGull.GetComponent<Seagull>().home = this.gameObject;
        newGull.GetComponent<Seagull>().cam = cam;
        newGull.transform.forward = transform.position + target - spawnWorldPos;
        newGull.GetComponent<Seagull>().flying = true;
        newGull.GetComponent<Seagull>().flyDirection = newGull.transform.forward;
        seagulls.Add(newGull);

        currentNumSeagulls++;
    }

    //Will add seagulls at the start of the scene in a way that they don't fly in
    void InitPopulation()
    {
        while(currentNumSeagulls < maxSeagulls)
        {
            Vector3 localPos = FindSpawnLocation();
            Vector3 rotEulers = new Vector3(0, Random.Range(0, 360f), 0);
            GameObject newGull = Object.Instantiate(seagullPrefab, localPos + transform.position, Quaternion.Euler(rotEulers));
            newGull.GetComponent<Seagull>().cam = cam;
            newGull.GetComponent<Seagull>().home = this.gameObject;
            seagulls.Add(newGull);

            currentNumSeagulls++;
        }
    }

    void Scatter(Vector3 offendingVector)
    {
        offendingVector.y = 0;
        offendingVector.Normalize();

        foreach(GameObject gull in seagulls)
        {
            gull.GetComponent<Seagull>().FlyAway(offendingVector);
        }

        seagulls.Clear();
        currentNumSeagulls = 0;
    }

    //finds a random spot within the radius, and casts downwards from the top of the sphere collider to find ground within it.
    Vector3 FindSpawnLocation()
    {
        RaycastHit hit;
        Vector3 localPos;
        bool overlapping = false;

        int loopCount = 0;

        do
        {
            localPos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * Random.Range(0.5f, radius);
            if (Physics.Raycast(transform.position + localPos + (Vector3.up * radius), Vector3.down, out hit, Mathf.Infinity, groundMask))
            {
                localPos = hit.point - transform.position;
            }

            //checks if any gulls overlap with the current intended spawn location
            foreach (GameObject gull in seagulls)
            {
                if (Vector3.Distance(gull.transform.position, localPos + transform.position) < 1f) //double the radius of the gull collider plus some
                    overlapping = true;
            }

            //infinite loop prevention
            loopCount++;
            if (loopCount > 100)
                break;

        } while (overlapping);

        return localPos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 && !fearless)
        {
            //sends through the vector of crock towards the center of the spawner
            Scatter(transform.position - other.transform.position);
        }
    }

    bool IsInView(Vector3 position)
    {
        Vector3 pos = cam.WorldToScreenPoint(position);
        if ((pos.x >= 0 - tolerance && pos.x <= Screen.width + tolerance) && (pos.y >= 0 - tolerance && pos.y <= Screen.height + tolerance))
        {
            return true;
        }
        return false;

    }

    public float GetRadius()
    {
        return radius;
    }
}
