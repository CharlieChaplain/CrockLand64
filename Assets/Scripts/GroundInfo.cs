//contains information about the ground it is attached to, like what type it is for footstep purposes or the friction coefficient

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundInfo : MonoBehaviour
{
    public enum GroundTypes
    {
        dirt,
        sand,
        stone,
        wood
    }

    public GroundTypes groundType;

    public float mu; //coeffecient of friction
}
