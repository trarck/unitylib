#if !(UNITY_EDITOR && UNITY_EDITOR_WIN)
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FastFileSystem
{
    public partial class FastFileSearch
    {
        /// <summary>
        /// 获取指定扩展名的文件路径
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="fileExtension"></param>
        /// <param name="files"></param>
        /// <returns>文件全路径列表</returns>
        public static int GetFilesNative(string searchDir, string fileExtension, bool recursive, ICollection<string> files)
        {
            if (!Directory.Exists(searchDir))
            {
                return 0;
            }
            foreach (var file in Directory.EnumerateFiles(searchDir, fileExtension, 
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                files.Add(file.Replace("\\", "/"));
            }
            return 0;
        }

        /// <summary>
        /// 获取指定扩展名的文件路径
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="fileExtension"></param>
        /// <param name="relativePath"></param>
        /// <param name="files"></param>
        /// <returns>文件相对路径列表</returns>
        public static int GetFilesNative(string searchDir, string fileExtension, string relativePath,bool recursive,  ICollection<string> files)
        {
            if (!Directory.Exists(searchDir))
            {
                return 0;
            }
            int searchDirLen = searchDir.Length;
            foreach(var filePath in Directory.EnumerateFiles(searchDir, fileExtension, 
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                string fileRelativePath = string.Concat(relativePath, filePath.Substring(searchDirLen));
                files.Add(fileRelativePath.Replace("\\", "/"));
            }
            return 0;
        }

        /// <summary>
        /// 获取文件名包含提定内容的文件
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="searchPattern"></param>
        /// <param name="relativePath"></param>
        /// <param name="files"></param>
        /// <returns>文件相对路径列表</returns>
        public static int SearchFilesNative(string searchDir, string searchPattern, string relativePath, bool recursive, ICollection<string> files)
        {
            if (!Directory.Exists(searchDir))
            {
                return 0;
            }
            int searchDirLen = searchDir.Length;
            foreach (var filePath in Directory.EnumerateFiles(searchDir, "*",
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                if (!filePath.Contains(searchPattern))
                {
                    continue;
                }
                string fileRelativePath = string.Concat(relativePath, filePath.Substring(searchDirLen));
                files.Add(fileRelativePath.Replace("\\", "/"));
            }
            return 0;
        }

        /// <summary>
        /// 自定义文件过虑方法
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="filterFun"></param>
        /// <param name="relativePath"></param>
        /// <param name="files"></param>
        /// <returns>文件相对路径列表</returns>
        public static int SearchFilesNative(string searchDir, Func<string, string, bool> filterFun, string relativePath, bool recursive, ICollection<string> files)
        {
            if (!Directory.Exists(searchDir))
            {
                return 0;
            }
            int searchDirLen = searchDir.Length;
            foreach (var filePath in Directory.EnumerateFiles(searchDir, "*", 
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                if (!filterFun(filePath,null))
                {
                    continue;
                }
                string fileRelativePath = string.Concat(relativePath, filePath.Substring(searchDirLen));
                files.Add(fileRelativePath.Replace("\\", "/"));
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="regexPattern"></param>
        /// <param name="relativePath"></param>
        /// <param name="files"></param>
        /// <returns>文件相对路径列表</returns>
        public static int SearchFilesRegexNative(string searchDir, string namePattern, string pathPattern, string relativePath, bool recursive, ICollection<string> files)
        {
            if (!Directory.Exists(searchDir))
            {
                return 0;
            }

            int searchDirLen = searchDir.Length;

            Regex regexName = null;
            if (!string.IsNullOrEmpty(namePattern))
            {
                regexName = new Regex(namePattern, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            }

            Regex regexPath = null;
            if (!string.IsNullOrEmpty(pathPattern))
            {
                regexPath = new Regex(pathPattern, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            }

            foreach (var filePath in Directory.EnumerateFiles(searchDir, "*", 
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {

                if (regexName!=null)
                {
                    string fileName = GetFileName(filePath);
                    if (!regexName.IsMatch(fileName))
                    {
                        continue;
                    }
                }
                string fileRelativePath = string.Concat(relativePath, filePath.Substring(searchDirLen).Replace("\\","/"));
                if(regexPath!=null)
                {
                     if(!regexPath.IsMatch(fileRelativePath))
                    {
                        continue;
                    }
                }

                files.Add(fileRelativePath);
            }
            return 0;
        }
    }
}
#endif