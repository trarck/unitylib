using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using YH.AM;
public class MyEditor : Editor
{
    [MenuItem("MyMenu/Test AM")]
    public static void Test()
    {

        GeneratePatch gen = new GeneratePatch();
        gen.Generate("D:\\temp\\am\\iphone");
    }

    static bool IsVersionFormat(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        foreach (char c in name)
        {
            if (!char.IsNumber(c) && c!='.')
            {
                return false;
            }
        }

        return true;
    }
}
