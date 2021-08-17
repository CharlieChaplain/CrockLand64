using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doubloon : Wealth
{
    [Header("To Disable")]
    public SpriteRenderer sprite;

    protected override void ToggleModel(bool visibility)
    {
        sprite.enabled = visibility;
    }
}
