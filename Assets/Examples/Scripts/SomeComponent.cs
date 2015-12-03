using UnityEngine;
using System.Collections;
using UnityEditor;

public class SomeComponent : MonoBehaviour
{
    public ChangeColor SomeColor;
    public ChangeScore SomeScore;
}


[System.Serializable]
public class ChangeColor
{
    public Material Mat;
    public Color To;

    public string[] paths;
}

[System.Serializable]
public class ChangeScore
{
    public int Points;
    public Transform Position;
}
