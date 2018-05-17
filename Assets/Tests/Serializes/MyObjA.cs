using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObjA : MonoBehaviour {

    [SerializeField]
    int m_Age;

    [SerializeField]
    string m_MyName;

    public int testa;

    [System.NonSerialized]
    public int testb;

    int m_MyNormal;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
