using UnityEngine;
using System.Collections;

public class TestSingletonB : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        Debug.Log("in b");
        TestManager.Instance.DoSome();
        TestManager.Instance.a = 3;

        TestManager.Instance.DoSome();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
