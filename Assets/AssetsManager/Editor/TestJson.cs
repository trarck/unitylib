using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class TestJson :  Editor
{
    [MenuItem("MyMenu/Test Json")]
    public static void Test()
    {
        MyTestObject myObject = new MyTestObject();
        myObject.name = "雨松MOMO";
        myObject.TT = "test";
        myObject.newOjbect = new MyTestNewObject() { level = 100 };

        string json = JsonUtility.ToJson(myObject);
        Debug.Log(json);

        myObject = JsonUtility.FromJson<MyTestObject>(json);
        Debug.Log(myObject.name + " " + myObject.newOjbect.level);

        JsonUtility.FromJsonOverwrite(json, myObject);
    }
}

[Serializable]
public class MyTestObject
{
    public string name;
    public MyTestNewObject newOjbect;

    [SerializeField]
    string m_TT;
    public string TT
    {
        get
        {
            return m_TT;
        }

        set
        {
            m_TT = value;
        }
    }
}
[Serializable]
public class MyTestNewObject
{
    public int level;
}