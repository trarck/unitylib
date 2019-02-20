using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace YH
{
    public class FileSystemUtil
    {
        public static void ForceDeleteDirectory(string path)
        {
            DirectoryInfo root;
            Stack<DirectoryInfo> fols;
            DirectoryInfo fol;
            fols = new Stack<DirectoryInfo>();
            root = new DirectoryInfo(path);
            fols.Push(root);
            while (fols.Count > 0)
            {
                fol = fols.Pop();
                fol.Attributes = fol.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                foreach (DirectoryInfo d in fol.GetDirectories())
                {
                    fols.Push(d);
                }
                foreach (FileInfo f in fol.GetFiles())
                {
                    f.Attributes = f.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                    f.Delete();
                }
            }
            root.Delete(true);
        }

        public static void ForceClearDirectory(string path)
        {
            DirectoryInfo root;
            Stack<DirectoryInfo> fols;
            DirectoryInfo fol;
            fols = new Stack<DirectoryInfo>();
            root = new DirectoryInfo(path);
            fols.Push(root);
            while (fols.Count > 0)
            {
                fol = fols.Peek();
                fol.Attributes = fol.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);

                foreach (FileInfo f in fol.GetFiles())
                {
                    f.Attributes = f.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                    f.Delete();
                }

                DirectoryInfo[] subs = fol.GetDirectories();
                if (subs.Length > 0)
                {
                    foreach (DirectoryInfo d in fol.GetDirectories())
                    {
                        fols.Push(d);
                    }
                }
                else
                {
                    if (fol != root)
                    {
                        fol.Delete(true);
                    }

                    fols.Pop();
                }
            }
        }

        public static void RemoveDirectoryFiles(string path, string pattern)
        {
            DirectoryInfo root;
            Stack<DirectoryInfo> fols;
            DirectoryInfo fol;
            fols = new Stack<DirectoryInfo>();
            root = new DirectoryInfo(path);
            fols.Push(root);
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            while (fols.Count > 0)
            {
                fol = fols.Peek();
                foreach (FileInfo f in fol.GetFiles())
                {
                    Debug.Log(f.Name + ":" + reg.IsMatch(f.Name));

                    if (reg.IsMatch(f.Name))
                    {
                        f.Attributes = f.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                        f.Delete();
                    }
                }

                DirectoryInfo[] subs = fol.GetDirectories();
                if (subs.Length > 0)
                {
                    foreach (DirectoryInfo d in fol.GetDirectories())
                    {
                        fols.Push(d);
                    }
                }
                else
                {
                    fols.Pop();
                }
            }
        }

        public static string Relative(string fromPath, string toPath)
        {
            fromPath = fromPath.Replace("\\", "/");
            toPath = toPath.Replace("\\", "/");

            if (fromPath[fromPath.Length - 1] == '/')
            {
                fromPath = fromPath.Substring(0, fromPath.Length - 1);
            }

            if (toPath[toPath.Length - 1] == '/')
            {
                toPath = toPath.Substring(0, toPath.Length - 1);
            }

            string[] froms = fromPath.Split('/');
            string[] tos = toPath.Split('/');

            int i = 0;
            //look for same part
            for (; i < froms.Length; ++i)
            {
                if (froms[i] != tos[i])
                {
                    break;
                }
            }

            if (i == 0)
            {
                //just windows. eg.fromPath=c:\a\b\c,toPath=d:\e\f\g
                //if linux the first is empty always same. eg. fromPath=/a/b/c,toPath=/d/e/f
                return toPath;
            }
            else
            {
                System.Text.StringBuilder result = new System.Text.StringBuilder();
                System.Text.StringBuilder toSB = new System.Text.StringBuilder();

                for (int j = i; j < froms.Length; ++j)
                {
                    result.Append("../");
                }

                for (int j = i; j < tos.Length; ++j)
                {
                    result.Append(tos[j]);
                    if (j < tos.Length - 1)
                    {
                        result.Append("/");
                    }
                }
                return result.ToString();
            }
        }
    }
}