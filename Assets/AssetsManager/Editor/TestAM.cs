using UnityEngine;
using System.Collections;
using UnityEditor;
using YH.AM;
public class MyEditor : Editor
{
    [MenuItem("MyMenu/Test AM")]
    public static void Test()
    {
        GenerateManifest gm = new GenerateManifest();
        gm.Process("d:\\temp\\am\\0.0.1","d:\\temp\\am\\0.0.2","");
        Debug.Log(gm.ToString());

       Manifest manifest= gm.GenManifest();
        string json = fastJSON.JSON.ToJSON(manifest.ToDictionary());
        Debug.Log(json);
    }
}
