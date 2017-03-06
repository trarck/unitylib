using UnityEngine;
using System.Collections;
using UnityEditor;
using YH.AM;
public class MyEditor : Editor
{
    [MenuItem("MyMenu/Test AM")]
    public static void Test()
    {

        //GenerateManifest gm = new GenerateManifest();
        //gm.Process("d:\\temp\\am\\0.0.1", "d:\\temp\\am\\0.0.2", "");
        //Debug.Log(gm.ToString());

        //Manifest manifest = gm.GenManifest();
        //string json = JsonUtility.ToJson(manifest);
        //Debug.Log(json);

        Version v1 = new Version("0.1.2");
        Version v2 = new Version("1.0.0");

        Debug.Log(v1.ToLong());
        Debug.Log(v2.ToLong());

        Debug.Log(v1>v2);

        Debug.Log(v1 <= v2);
    }
}
