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
        GeneratePatch gen = new GeneratePatch();
        gen.UseDiffPatch = true;
        gen.PatchBlackDirs = new List<string>() { "Lua" };
        gen.Generate("D:\\temp\\am\\iphone");
        //Debug.Log(Application.version);
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
        
    }

    [MenuItem("MyMenu/Test diff")]
    public static void TestDiff()
    {
        string src = "d:/temp/diff/Version.txt";
        string target = "d:/temp/diff/Version2.txt";
        string patchFile = "d:/temp/diff/patch.bin";
        using (FileStream output = new FileStream(patchFile, FileMode.Create))
        {
            BsDiff.BinaryPatchUtility.Create(File.ReadAllBytes(src), File.ReadAllBytes(target), output);
        }            
    }

    [MenuItem("MyMenu/Test patch")]
    public static void TestPatch()
    {
        string src = "d:/temp/diff/Version.txt";
        string target = "d:/temp/diff/Version3.txt";
        string patchFile = "d:/temp/diff/patch.bin";
        using (FileStream input = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (FileStream output = new FileStream(target, FileMode.Create))
        {
            BsDiff.BinaryPatchUtility.Apply(input, () => new FileStream(patchFile, FileMode.Open, FileAccess.Read, FileShare.Read), output);
        }            
    }

    [MenuItem("MyMenu/Test ttt2")]
    public static void TestTTT2()
    {
        string src = "d:/temp/diff/ttt2.txt";
        byte[] buff = new byte[4] { 0, 0, 0, 0};
        MemoryStream comressMem = new MemoryStream(255);
        using (FileStream fs = new FileStream(src, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        using (Ionic.BZip2.BZip2OutputStream bz2Stream = new Ionic.BZip2.BZip2OutputStream(fs, true))
        {
            bz2Stream.Write(buff, 0, 4);
        }

        byte[] data = new byte[255];
        using (FileStream fs = new FileStream(src, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        using (Ionic.BZip2.BZip2InputStream bz2Stream = new Ionic.BZip2.BZip2InputStream(fs,true))
        {
            bz2Stream.Read(data, 0, 255);
        }

        string s = System.Text.Encoding.Default.GetString(data);
        Debug.Log(s);
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
