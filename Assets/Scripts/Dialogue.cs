using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea(3,10)]
    public string[] sentences;

    public GameObject[] cameras;
    public Sprite[] portraits;

    public AudioClip[] gobbledyGook; //this will be filled out later when they are recorded. Used by the Dialogue Manager to play random sounds as someone talks

    public NPC npc;
}
