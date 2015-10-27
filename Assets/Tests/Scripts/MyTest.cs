using UnityEngine;

public class MyTest
{
    private static readonly MyTest a = new MyTest();

    static MyTest()
    {
        Debug.Log("In static MyTest");
    }

    private MyTest()
    {
        Debug.Log("in MyTest");
    }
    public static MyTest Instance
    {
        get
        {
            return a;
        }
    }
}
