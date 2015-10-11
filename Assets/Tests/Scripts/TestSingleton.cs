using UnityEngine;
using System.Collections;
using YH;

public class TestSingleton : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        Debug.Log("in a");

        TestManager.Instance.DoSome();
        TestManager.Instance.a = 2;

        TestManager.Instance.DoSome();

        Application.LoadLevel("SingletonB");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
