using UnityEngine;
using System.Collections;

public class TestManager : Singleton<TestManager> {

    public int a = 0;

    public void DoSome()
    {
        Debug.Log("this is DoSome a = " + a);
    }
}
