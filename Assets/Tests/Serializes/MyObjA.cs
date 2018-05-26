using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubObj
{
    [SerializeField]
    int[] tests;

    [SerializeField]
    string myName;

    [SerializeField]
    int myAge;
}

public class MyObjA : MonoBehaviour {

    [SerializeField]
    int m_Age;

    [SerializeField]
    string m_MyName;

    public int testa;

    [System.NonSerialized]
    public int testb;

    [SerializeField]
    int[] indexs;

    [SerializeField]
    SubObj[] objs;

    [SerializeField]
    SubObj m_Sub;

    int m_MyNormal;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
