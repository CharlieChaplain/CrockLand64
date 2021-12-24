using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleTrapped_Logic : Enemy
{
    bool freed = false;

    public SkinnedMeshRenderer model;
    public Mesh unboundMesh;
    public ParticleSystem ropePart;
    public ParticleSystem dirtPart;

    BoxCollider col;
    SphereCollider bounceCol;

    public Animator moleHillAnim;

    public string moleID; //a 2 digit string representing the level the mole is in and which mole it is.
    public MoleUI moleUI; //the corresponding UI element of the mole

    public PlaySound[] struggleSounds;
    public PlaySound freeSound;
    float struggleTimer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        controller = GetComponent<CharacterController>();
        col = GetComponent<BoxCollider>();
        bounceCol = GetComponent<SphereCollider>();

        if (!int.TryParse(moleID, out int errorCatcher))
            Debug.Log("MoleLogic: MOLE ID " + moleID + " IS INVALID AND WILL THROW ERRORS");
        else
        {
            moleUI.moleID = moleID;
            if (TreasureMaster.Instance.QueryMole(int.Parse(moleID.Substring(0, 1)), int.Parse(moleID.Substring(1, 1))))
                Destroy(gameObject);
        }

        struggleTimer = Random.Range(2f, 5f);
    }

    // Update is called once per frame
    protected override void Update()
    {
        struggleTimer -= Time.deltaTime;
        if(struggleTimer <= 0 && !freed)
        {
            struggleTimer = Random.Range(2f, 5f);
            struggleSounds[Random.Range(0, 3)].Play(transform.position);
        }
    }

    void Freed()
    {
        freed = true;
        TreasureMaster.Instance.RescueMole(int.Parse(moleID.Substring(0, 1)), int.Parse(moleID.Substring(1, 1)));
        TreasureMaster.Instance.Save();
        moleUI.SaveMole();

        anim.SetTrigger("BreakFree");
        moleHillAnim.SetTrigger("BreakFree");

        //swaps model out and replaces materials. (Otherwise a bug would occur where the top two materials would stack on top of eachother
        Material[] mats = new Material[2];
        mats[0] = model.materials[0];
        mats[1] = model.materials[1];
        model.sharedMesh = unboundMesh;
        model.materials = mats;

        ropePart.Play();
        freeSound.Play(transform.position);

        StartCoroutine("Leave");
    }

    private void OnTriggerEnter(Collider other)
    {
        int layerNumber = 9; //layer 9 = player
        if (other.gameObject.layer == layerNumber && other.CompareTag("HurtBox"))
        {
            PlaySound hurtSound = null;
            if (other.gameObject.layer == 9) //layer 9 = player
                hurtSound = PlayerManager.Instance.currentHitSound;
            Hurt(other.GetComponent<HurtBoxInfo>().damage, other.transform.position, hurtSound);

            if (health <= 0 && !freed)
            {
                Freed();
            }
        }
    }

    IEnumerator Leave()
    {
        yield return new WaitForSeconds(3.15f);

        dirtPart.Play();

        float timer = 3f;
        //moves the colliders down every frame til all of them and the model are below the ground
        Vector3 increment = Vector3.up * 0.02f;
        while (timer > 0)
        {
            controller.center -= increment;
            col.center -= increment;
            bounceCol.center -= increment;

            timer -= Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
