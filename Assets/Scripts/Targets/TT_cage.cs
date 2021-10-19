using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TT_cage : TargetTarget
{
    Cutscene cutscene;

    private void Start()
    {
        anim = GetComponent<Animator>();
        cutscene = GetComponent<Cutscene>();
    }

    public override void Trigger()
    {
        cutscene.PlayCutscene();
    }
}
