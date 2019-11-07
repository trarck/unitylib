using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace YHEditor
{
    public class FindAsset
    {
        /// <summary>
        /// 使用AssetDatabase来查找
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<string> FindAllAssets(string path,string filter)
        {
            List<string> result = new List<string>();

            string[] allGuids = AssetDatabase.FindAssets(filter, new string[] { path });
            for(int i = 0; i < allGuids.Length; ++i)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allGuids[i]);
                result.Add(assetPath);
            }

            return result;
        }

        /// <summary>
        /// 使用文件系统来搜索
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern">正则表达式</param>
        /// /// <returns></returns>
        public static List<string> SearchAllAssets(string path, string pattern = null)
        {
            DirectoryInfo startInfo = new DirectoryInfo(path);
            if (!startInfo.Exists)
            {
                return null;
            }

            List<string> assets = new List<string>();

            Stack<DirectoryInfo> dirs = new Stack<DirectoryInfo>();
            dirs.Push(startInfo);

            DirectoryInfo dir;

            bool haveFilter = false;
            System.Text.RegularExpressions.Regex reg = null;
            if (!string.IsNullOrEmpty(pattern))
            {
                haveFilter = true;
                reg = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            string applicationPath = Path.GetDirectoryName(Application.dataPath);
            while (dirs.Count > 0)
            {
                dir = dirs.Pop();

                foreach (FileInfo fi in dir.GetFiles())
                {
                    if (!haveFilter || reg.IsMatch(fi.FullName))
                    {
                        assets.Add(YH.FileSystem.Relative(applicationPath, fi.FullName));
                    }
                }

                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    if (!subDir.Name.StartsWith("."))
                    {
                        dirs.Push(subDir);
                    }
                }
            }
            return assets;
        }

    }
}