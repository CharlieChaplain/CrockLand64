using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BogeyAnimListener : MonoBehaviour
{
    Bogey_Logic logic;

    private void Start()
    {
        logic = transform.root.gameObject.GetComponent<Bogey_Logic>();
    }

    //turns on attacking bool in nutter_logic
    public void ToggleAttack(int flag)
    {
        if (flag == 1)
            logic.attacking = true;
        else
            logic.attacking = false;
    }

    public void ToggleHurtBox(int flag)
    {
        if (flag == 1)
            logic.ToggleHurtBox(true);
        else
            logic.ToggleHurtBox(false);
    }

    public void SetFelled(int felled)
    {
        logic.SetFelled(felled == 1);
    }
}
