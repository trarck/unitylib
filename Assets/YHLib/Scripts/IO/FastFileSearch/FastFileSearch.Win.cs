#if UNITY_EDITOR && UNITY_EDITOR_WIN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FastFileSystem
{
    public partial class FastFileSearch
    {
        struct SearchItem
        {
            public string dirPath;
            public string relativePath;

            public SearchItem(string dirPath, string relativePath)
            {
                this.dirPath = dirPath;
                this.relativePath = relativePath;
            }
        }

        private static string FixPattern(string fileExtension)
        {
            if (fileExtension[0] == '*')
            {
                if (fileExtension.Length == 1)
                {
                    fileExtension = string.Empty;
                }
                else
                {
                    fileExtension = fileExtension.Substring(1);
                }
            }
            return fileExtension;
        }

        private static bool NeedSkip(string fileName)
        {
            return string.IsNullOrEmpty(fileName) || fileName[0] == '.' && (fileName.Length == 1 || fileName[1] == '.');
        }

        /// <summary>
        /// 获取指定扩展名的文件路径
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="fileExtension"></param>
        /// <param name="files"></param>
        /// <returns>文件全路径列表</returns>
        public static int GetFilesNative(string searchDir, string fileExtension, bool recursive, ICollection<string> files)
        {
            Kernel32FileApi.WIN32_FIND_DATAW ffd;
            IntPtr hFind = Kernel32FileApi.INVALID_HANDLE_VALUE;

            string currentDir;
            string currentDirPathWithAll;

            if (string.IsNullOrEmpty(fileExtension))
            {
                return -1;
            }

            fileExtension = FixPattern(fileExtension);

            Stack<string> needSearchDirs = new Stack<string>();
            needSearchDirs.Push(searchDir);

            Stack<string> tempDirs = new Stack<string>();

            while (needSearchDirs.Count() > 0)
            {
                currentDir = needSearchDirs.Pop();

                currentDirPathWithAll = string.Concat(currentDir ,"/*");
                hFind = Kernel32FileApi.FindFirstFileW(currentDirPathWithAll, out ffd);

                if (Kernel32FileApi.INVALID_HANDLE_VALUE == hFind)
                {
                    continue;
                }

                tempDirs.Clear();
                do
                {
                    if (NeedSkip(ffd.cFileName))
                    {
                        continue;
                    }

                    if ((ffd.dwFileAttributes & Kernel32FileApi.FILE_ATTRIBUTE_DIRECTORY)!=0)
                    {
                        if(recursive)
                            tempDirs.Push(string.Concat(currentDir, "/", ffd.cFileName));
                    }
                    else
                    {
                        if (EndsWith(ffd.cFileName,fileExtension))
                        {
                            files.Add(string.Concat(currentDir, "/", ffd.cFileName));
                        }
                    }
                } while (Kernel32FileApi.FindNextFileW(hFind, out ffd));
                Kernel32FileApi.FindClose(hFind);

                while (tempDirs.Count > 0)
                {
                    needSearchDirs.Push(tempDirs.Pop());
                }
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
        public static int GetFilesNative(string searchDir, string fileExtension, string relativePath, bool recursive, ICollection<string> files)
        {
            Kernel32FileApi.WIN32_FIND_DATAW ffd;
            IntPtr hFind = Kernel32FileApi.INVALID_HANDLE_VALUE;

            SearchItem currentDir;
            string currentDirPathWithAll;

            if (string.IsNullOrEmpty(fileExtension))
            {
                return -1;
            }

            fileExtension = FixPattern(fileExtension);

            Stack<SearchItem> needSearchDirs = new Stack<SearchItem>();
            needSearchDirs.Push(new SearchItem(searchDir,relativePath));

            Stack<SearchItem> tempDirs = new Stack<SearchItem>();

            while (needSearchDirs.Count() > 0)
            {
                currentDir = needSearchDirs.Pop();
                currentDirPathWithAll = string.Concat(currentDir.dirPath, "/*");
                hFind = Kernel32FileApi.FindFirstFileW(currentDirPathWithAll, out ffd);

                if (Kernel32FileApi.INVALID_HANDLE_VALUE == hFind)
                {
                    continue;
                }

                tempDirs.Clear();
                do
                {
                    if (NeedSkip(ffd.cFileName))
                    {
                        continue;
                    }

                    if ((ffd.dwFileAttributes & Kernel32FileApi.FILE_ATTRIBUTE_DIRECTORY) != 0)
                    {
                        if (recursive)
                        {
                            SearchItem item = new SearchItem(
                                string.Concat(currentDir.dirPath, "/", ffd.cFileName),
                                string.IsNullOrEmpty(currentDir.relativePath) ? ffd.cFileName : string.Concat(currentDir.relativePath, "/", ffd.cFileName)
                            );
                            tempDirs.Push(item);
                        }
                    }
                    else
                    {
                        if (EndsWith(ffd.cFileName, fileExtension))
                        {
                            files.Add(string.IsNullOrEmpty(currentDir.relativePath) ? ffd.cFileName : string.Concat(currentDir.relativePath, "/", ffd.cFileName));
                        }
                    }
                } while (Kernel32FileApi.FindNextFileW(hFind, out ffd));
                Kernel32FileApi.FindClose(hFind);

                while (tempDirs.Count > 0)
                {
                    needSearchDirs.Push(tempDirs.Pop());
                }
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
            Kernel32FileApi.WIN32_FIND_DATAW ffd;
            IntPtr hFind = Kernel32FileApi.INVALID_HANDLE_VALUE;

            SearchItem currentDir;
            string currentDirPath;
            string currentRelativePath;

            if (!string.IsNullOrEmpty(searchPattern))
            {
                searchPattern = FixPattern(searchPattern);
            }

            Stack<SearchItem> needSearchDirs = new Stack<SearchItem>();
            needSearchDirs.Push(new SearchItem(searchDir, relativePath));

            Stack<SearchItem> tempDirs = new Stack<SearchItem>();

            while (needSearchDirs.Count() > 0)
            {
                currentDir = needSearchDirs.Pop();
                currentDirPath = string.Concat(currentDir.dirPath, "/*");
                hFind = Kernel32FileApi.FindFirstFileW(currentDirPath, out ffd);

                if (Kernel32FileApi.INVALID_HANDLE_VALUE == hFind)
                {
                    continue;
                }

                currentRelativePath = string.IsNullOrEmpty(currentDir.relativePath) ? string.Empty : string.Concat(currentDir.relativePath, "/");

                tempDirs.Clear();

                do
                {
                    if (NeedSkip(ffd.cFileName))
                    {
                        continue;
                    }

                    if ((ffd.dwFileAttributes & Kernel32FileApi.FILE_ATTRIBUTE_DIRECTORY) != 0)
                    {
                        if (recursive)
                        {
                            SearchItem item = new SearchItem(
                                string.Concat(currentDir.dirPath, "/", ffd.cFileName),
                                string.Concat(currentRelativePath, ffd.cFileName)
                            );
                            tempDirs.Push(item);
                        }
                    }
                    else
                    {
                        if (ffd.cFileName.Contains(searchPattern) || currentRelativePath.Contains(searchPattern))
                        {
                            files.Add(string.Concat(currentRelativePath, ffd.cFileName));
                        }
                    }
                } while (Kernel32FileApi.FindNextFileW(hFind, out ffd));
                Kernel32FileApi.FindClose(hFind);

                while (tempDirs.Count > 0)
                {
                    needSearchDirs.Push(tempDirs.Pop());
                }
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
            Kernel32FileApi.WIN32_FIND_DATAW ffd;
            IntPtr hFind = Kernel32FileApi.INVALID_HANDLE_VALUE;

            SearchItem currentDir;
            string currentDirPathWithAll;
            string currentRelativePath;

            Stack<SearchItem> needSearchDirs = new Stack<SearchItem>();
            needSearchDirs.Push(new SearchItem(searchDir, relativePath));

            Stack<SearchItem> tempDirs = new Stack<SearchItem>();

            while (needSearchDirs.Count() > 0)
            {
                currentDir = needSearchDirs.Pop();
                currentDirPathWithAll = string.Concat(currentDir.dirPath, "/*");
                hFind = Kernel32FileApi.FindFirstFileW(currentDirPathWithAll, out ffd);

                if (Kernel32FileApi.INVALID_HANDLE_VALUE == hFind)
                {
                    continue;
                }

                currentRelativePath = string.IsNullOrEmpty(currentDir.relativePath) ? string.Empty : string.Concat(currentDir.relativePath, "/");

                do
                {
                    if (NeedSkip(ffd.cFileName))
                    {
                        continue;
                    }

                    if ((ffd.dwFileAttributes & Kernel32FileApi.FILE_ATTRIBUTE_DIRECTORY) != 0)
                    {
                        if (recursive)
                        {
                            SearchItem item = new SearchItem(
                                string.Concat(currentDir.dirPath, "/", ffd.cFileName),
                                string.Concat(currentRelativePath, ffd.cFileName)
                            );
                            tempDirs.Push(item);
                        }
                    }
                    else
                    {
                        if (filterFun==null || filterFun(ffd.cFileName, currentRelativePath))
                        {
                            files.Add(string.Concat(currentRelativePath, ffd.cFileName));
                        }
                    }
                } while (Kernel32FileApi.FindNextFileW(hFind, out ffd));
                Kernel32FileApi.FindClose(hFind);

                while (tempDirs.Count > 0)
                {
                    needSearchDirs.Push(tempDirs.Pop());
                }
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
            Kernel32FileApi.WIN32_FIND_DATAW ffd;
            IntPtr hFind = Kernel32FileApi.INVALID_HANDLE_VALUE;

            SearchItem currentDir;
            string currentDirPathWithAll;
            string currentRelativePath;

            Stack<SearchItem> needSearchDirs = new Stack<SearchItem>();
            needSearchDirs.Push(new SearchItem(searchDir, relativePath));
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

            Stack<SearchItem> tempDirs = new Stack<SearchItem>();

            while (needSearchDirs.Count() > 0)
            {
                currentDir = needSearchDirs.Pop();

                currentDirPathWithAll = string.Concat(currentDir.dirPath, "/*");
                hFind = Kernel32FileApi.FindFirstFileW(currentDirPathWithAll, out ffd);

                if (Kernel32FileApi.INVALID_HANDLE_VALUE == hFind)
                {
                    continue;
                }
                
                currentRelativePath = string.IsNullOrEmpty(currentDir.relativePath) ? string.Empty : string.Concat(currentDir.relativePath, "/");

                do
                {
                    if (NeedSkip(ffd.cFileName))
                    {
                        continue;
                    }

                    if ((ffd.dwFileAttributes & Kernel32FileApi.FILE_ATTRIBUTE_DIRECTORY) != 0)
                    {
                        if (recursive)
                        {
                            SearchItem item = new SearchItem(
                                string.Concat(currentDir.dirPath, "/", ffd.cFileName),
                                string.Concat(currentRelativePath, ffd.cFileName)
                            );
                            tempDirs.Push(item);
                        }
                    }
                    else
                    {
                        if ((regexName==null || regexName.IsMatch(ffd.cFileName)) && (regexPath==null || regexPath.IsMatch(currentRelativePath)))
                        {
                            files.Add(string.Concat(currentRelativePath, ffd.cFileName));
                        }
                    }
                } while (Kernel32FileApi.FindNextFileW(hFind, out ffd));
                Kernel32FileApi.FindClose(hFind);

                while (tempDirs.Count > 0)
                {
                    needSearchDirs.Push(tempDirs.Pop());
                }
            }
            return 0;
        }
    }
}
#endif