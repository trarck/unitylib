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
        public static int GetFilesNative(string searchDir, string fileExtension, ICollection<string> files)
        {
            foreach(var file in Directory.EnumerateFiles(searchDir, fileExtension, SearchOption.AllDirectories))
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
        public static int GetFilesNative(string searchDir, string fileExtension, string relativePath, ICollection<string> files)
        {
            int searchDirLen = searchDir.Length;
            foreach(var filePath in Directory.EnumerateFiles(searchDir, fileExtension, SearchOption.AllDirectories))
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
        public static int SearchFilesNative(string searchDir, string searchPattern, string relativePath, ICollection<string> files)
        {
            int searchDirLen = searchDir.Length;
            foreach (var filePath in Directory.EnumerateFiles(searchDir, "*", SearchOption.AllDirectories))
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
        public static int SearchFilesNative(string searchDir, Func<string, string, bool> filterFun, string relativePath, ICollection<string> files)
        {
            int searchDirLen = searchDir.Length;
            foreach (var filePath in Directory.EnumerateFiles(searchDir, "*", SearchOption.AllDirectories))
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
        public static int SearchFilesRegexNative(string searchDir, string regexPattern, string relativePath, ICollection<string> files)
        {
            int searchDirLen = searchDir.Length;
            Regex regex = new Regex(regexPattern);
            foreach (var filePath in Directory.EnumerateFiles(searchDir, "*", SearchOption.AllDirectories))
            {
                if (!regex.IsMatch(filePath))
                {
                    continue;
                }
                string fileRelativePath = string.Concat(relativePath, filePath.Substring(searchDirLen));
                files.Add(fileRelativePath.Replace("\\", "/"));
            }
            return 0;
        }
    }
}
#endif