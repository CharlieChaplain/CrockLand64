using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Treasure/TreasureInfo Object")]

public class TreasureInfo : ScriptableObject
{
    new public string name;

    [TextArea(15, 20)]
    public string description;

    public bool collected = false;
}
