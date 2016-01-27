using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bone  {
    public int dimensiton;
    public Vector3 position;
    public Quaternion quaternion;
    public byte IsTracking;
}

public class Human
{
    public int id;
    public int numofBone;
    public Bone[] bones;

    
}

public class LRFdataSet
{
    public int id;
    public Vector3 pos;
}

