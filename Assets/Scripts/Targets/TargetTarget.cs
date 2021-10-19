using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the target of a target block. Will do something when the target is hit with an enemy.
/// </summary>
public abstract class TargetTarget : MonoBehaviour
{
    public Animator anim;
    public abstract void Trigger();
}
