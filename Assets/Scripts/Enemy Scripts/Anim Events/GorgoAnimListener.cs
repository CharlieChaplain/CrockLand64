using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorgoAnimListener : MonoBehaviour
{
     Gorgo_Logic logic;

    private void Start()
    {
        logic = transform.root.gameObject.GetComponent<Gorgo_Logic>();
    }

    //turns on attacking bool in gorgo_logic
    public void ToggleAttack(int flag)
    {
        if (flag == 1)
            logic.attacking = true;
        else
            logic.attacking = false;
    }

    public void SetFelled(int felled)
    {
        logic.SetFelled(felled == 1);
    }

    public void ChangeTex(int index)
    {
        logic.ChangeTex(index);
    }

    public void ActivateAura()
    {
        logic.ActivateAura();
    }
}
