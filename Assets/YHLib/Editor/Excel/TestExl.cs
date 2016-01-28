using UnityEngine;
using UnityEditor;
using System.IO;

using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

public class MySTream: FileStream
{
    public MySTream(string path, FileMode mode):base(path,mode)
    {

    }

    public override void Close()
    {
        base.Close();
        Debug.Log("MyStream Close");
    }

    //public MySTream(SafeFileHandle handle, FileAccess access);
    //public MySTream(IntPtr handle, FileAccess access);
    //public MySTream(SafeFileHandle handle, FileAccess access, int bufferSize);
    //public MySTream(string path, FileMode mode, FileAccess access);
    //public MySTream(IntPtr handle, FileAccess access, bool ownsHandle);
    //public MySTream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync);
    //public MySTream(string path, FileMode mode, FileAccess access, FileShare share);
    //public MySTream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize);
    //public MySTream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize);
    //public MySTream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync);
    //public MySTream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options);
    //public MySTream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options);
    //public MySTream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync);
    //public MySTream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity);
}

public static class TextExl {
 
    [MenuItem("Assets/Exl/Test")]
    public static void Test()
    {
        string fileName =Application.dataPath+ "/Tests/Editor/Gacha.xlsx";

        IWorkbook workbook = null;

        if (File.Exists(fileName))
        {
            //FileStream fs = File.OpenRead(fileName);
            MySTream fs = new MySTream(fileName, FileMode.Open);

            if (fileName.IndexOf(".xlsx") > 0)// ÐÂ°æ
            {
                Debug.Log("new");
                workbook = new XSSFWorkbook(fs);
            }
            else if (fileName.IndexOf(".xls") > 0)// ¾É°æ
            {
                Debug.Log("old");
                workbook = new HSSFWorkbook(fs);
            }

            Debug.Log(fs.CanRead);
        }
        else
        {
            Debug.Log("no file:" + fileName);
        }
    }   
}