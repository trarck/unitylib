using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using YH;

public class TestCustomYield : MonoBehaviour {

	// Use this for initialization
	void Start () {
        TestStart();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void TestStart()
    {
        StartCoroutine(TestA());
    }

    IEnumerator TestA()
    {
        var done = false;
        string result = "";
        Debug.Log("TestA Start-->"+Time.frameCount);
        AsyncFunc("AAA", (r) =>
        {
            Debug.Log("Callback Result:" + r + "-->" + Time.frameCount);
            result = r;
            done = true;            
        });

        while (done == false)
        {
            Debug.Log("In While "+ result+" -->" + Time.frameCount);
            yield return result;
        }
        Debug.Log("TestA End "+result+"-->" + Time.frameCount);
    }

    void AsyncFunc(string param,Action<string> callback)
    {
        Debug.Log("AsyncFunc Start with:"+param+ "-->" + Time.frameCount);
        StartCoroutine(AsyncBody(callback));
    }

    IEnumerator AsyncBody(Action<string> callback)
    {
        Debug.Log("AsyncBody Start-->" + Time.frameCount);
        yield return new WaitForSeconds(3);
        if (callback!=null)
        {
            callback("success");
        }
        Debug.Log("AsyncBody End-->" + Time.frameCount);
    }
}
