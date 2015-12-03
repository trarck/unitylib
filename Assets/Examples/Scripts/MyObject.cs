using UnityEngine;
using System.Collections;

public class MyObject : ScriptableObject
{
    public int myInt = 42;

    public string[] myArr;

    public SubObject subObj;
}

[System.Serializable]
public class SubObject
{
    public Material Mat;
    public Color To;

    public string name;

    public string[] paths;
}