using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using YH.AM;
using YH;
using Ionic.Zip;

public class MyEditor : Editor
{
    [MenuItem("MyMenu/Test AM")]
    public static void Test()
    {
        //GeneratePatch gen = new GeneratePatch();
        //gen.Generate("D:\\temp\\am\\iphone");
        Debug.Log(Application.version);
    }

    [MenuItem("MyMenu/Test zip")]
    public static void TestZip()
    {
        string patchPacktUrl = "http://patch.xuebaogames.com/monster/1.0.0_1.0.6.zip";
        
        HttpRequest httpRequest = new HttpRequest();

        EditorCoroutine.StartCoroutine((httpRequest.Request(patchPacktUrl, (err, www) =>
        {
            if (err != null)
            {
                Debug.Log(err);
            }
            else
            {
                byte[] bytes = www.bytes;

                MemoryStream ms = null;
                ZipFile zipFile = null;
                bool haveManifest = false;
                Manifest manifest = null;
                Dictionary<string, Asset> assetsMap = new Dictionary<string, Asset>();
                try
                {
                    ms= new MemoryStream(bytes);
                    zipFile = ZipFile.Read(ms);
                    //first is manifest file
                    foreach(ZipEntry zipEntry in zipFile)
                    {
                        Debug.Log(zipEntry.Info);
                        //Debug.Log(Path.Combine(Application.persistentDataPath, zipEntry.FileName));
                        zipEntry.Extract(Application.persistentDataPath, ExtractExistingFileAction.OverwriteSilently);                  
                    }                    
                }
                finally
                {

                    if (ms != null)
                    {
                        ms.Close();
                    }

                    if (zipFile != null)
                    {
                        zipFile.Dispose();
                    }

                    if (ms != null)
                    {
                        ms.Dispose();
                    }
                }
               
            }
        })),httpRequest);
    }

    [MenuItem("MyMenu/Test coro")]
    public static void TestCoro()
    {
        string patchPacktUrl = "http://patch.xuebaogames.com/monster/1.0.6.zip";

        HttpRequest httpRequest = new HttpRequest();

        EditorCoroutine.StartCoroutine(httpRequest.TestWait(),httpRequest);
        //SimpleEditorCoroutine.start(httpRequest.TestWait());
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
