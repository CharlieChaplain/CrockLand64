using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutterAnimListener : MonoBehaviour
{
    Nutter_Logic logic;

    private void Start()
    {
        logic = transform.root.gameObject.GetComponent<Nutter_Logic>();
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

    //triggers attack coroutine in 
    public void TriggerAttack()
    {
        logic.AttackCoroutine();
    }
}
