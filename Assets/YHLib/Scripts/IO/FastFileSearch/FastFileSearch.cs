using System;
using System.Collections.Generic;

namespace FastFileSystem
{
    public partial class FastFileSearch
    {
        public unsafe static bool EndsWith(string a, string b)
        {
            if (string.IsNullOrEmpty(b))
            {
                return true;
            }

            if (string.IsNullOrEmpty(a))
            {
                return false;
            }

            fixed (char* pa = a)
            fixed (char* pb = b)
            {
                char* pea = pa + a.Length - 1;
                char* peb = pb + b.Length - 1;
                do
                {
                    if (*pea-- != *peb--)
                    {
                        return false;
                    }
                }
                while (pea != pa && peb != pb);
            }

            return true;
        }

        internal unsafe static int IndexOfUnchecked(string str, char value, int startIndex, int count)
        {
            if (string.IsNullOrEmpty(str))
            {
                return -1;
            }

            fixed (char* ptrStrStart = str)
            {
                char* ptrStr = ptrStrStart + startIndex;
                char* ptrStrEnd;
                for (ptrStrEnd = ptrStr + (count >> 3 << 3); ptrStr != ptrStrEnd; ptrStr += 8)
                {
                    if (*ptrStr == value)
                    {
                        return (int)(ptrStr - ptrStrStart);
                    }
                    if (ptrStr[1] == value)
                    {
                        return (int)(ptrStr - ptrStrStart + 1);
                    }
                    if (ptrStr[2] == value)
                    {
                        return (int)(ptrStr - ptrStrStart + 2);
                    }
                    if (ptrStr[3] == value)
                    {
                        return (int)(ptrStr - ptrStrStart + 3);
                    }
                    if (ptrStr[4] == value)
                    {
                        return (int)(ptrStr - ptrStrStart + 4);
                    }
                    if (ptrStr[5] == value)
                    {
                        return (int)(ptrStr - ptrStrStart + 5);
                    }
                    if (ptrStr[6] == value)
                    {
                        return (int)(ptrStr - ptrStrStart + 6);
                    }
                    if (ptrStr[7] == value)
                    {
                        return (int)(ptrStr - ptrStrStart + 7);
                    }
                }
                for (ptrStrEnd += count & 7; ptrStr != ptrStrEnd; ptrStr++)
                {
                    if (*ptrStr == value)
                    {
                        return (int)(ptrStr - ptrStrStart);
                    }
                }
                return -1;
            }
        }

        internal unsafe static int IndexOfAnyUnchecked(string str, string anyOf, int startIndex, int count)
        {
            if (string.IsNullOrEmpty(str))
            {
                return -1;
            }

            if (string.IsNullOrEmpty(anyOf))
            {
                return -1;
            }

            if (anyOf.Length == 1)
            {
                return IndexOfUnchecked(str, anyOf[0], startIndex, count);
            }

            fixed (char* ptrAnyOfStart = anyOf)
            {
                int anyOfMax = *ptrAnyOfStart;
                int anyOfMin = *ptrAnyOfStart;
                char* ptrAnyOfEnd = ptrAnyOfStart + anyOf.Length;
                char* ptrAnyOf = ptrAnyOfStart;
                while (++ptrAnyOf != ptrAnyOfEnd)
                {
                    if (*ptrAnyOf > anyOfMax)
                    {
                        anyOfMax = *ptrAnyOf;
                    }
                    else if (*ptrAnyOf < anyOfMin)
                    {
                        anyOfMin = *ptrAnyOf;
                    }
                }
                fixed (char* ptrStrStart = str)
                {
                    char* ptrStr = ptrStrStart + startIndex;
                    char* ptrStrEnd = ptrStr + count;
                    while (ptrStr != ptrStrEnd)
                    {
                        if (*ptrStr > anyOfMax || *ptrStr < anyOfMin)
                        {
                            ptrStr++;
                            continue;
                        }
                        if (*ptrStr == *ptrAnyOfStart)
                        {
                            return (int)(ptrStr - ptrStrStart);
                        }
                        ptrAnyOf = ptrAnyOfStart;
                        while (++ptrAnyOf != ptrAnyOfEnd)
                        {
                            if (*ptrStr == *ptrAnyOf)
                            {
                                return (int)(ptrStr - ptrStrStart);
                            }
                        }
                        ptrStr++;
                    }
                }
            }
            return -1;
        }

        internal static unsafe int LastIndexOfUnchecked(string str, char value, int startIndex, int count)
        {
            fixed (char* ptrStrStart = str)
            {
                char* ptrStr = ptrStrStart + startIndex;
                char* ptrStrEnd = ptrStr - (count >> 3 << 3);
                while (ptrStr != ptrStrEnd)
                {
                    if (*ptrStr == value)
                    {
                        return (int)(ptrStr - ptrStrStart);
                    }
                    if (ptrStr[-1] == value)
                    {
                        return (int)(ptrStr - ptrStrStart) - 1;
                    }
                    if (ptrStr[-2] == value)
                    {
                        return (int)(ptrStr - ptrStrStart) - 2;
                    }
                    if (ptrStr[-3] == value)
                    {
                        return (int)(ptrStr - ptrStrStart) - 3;
                    }
                    if (ptrStr[-4] == value)
                    {
                        return (int)(ptrStr - ptrStrStart) - 4;
                    }
                    if (ptrStr[-5] == value)
                    {
                        return (int)(ptrStr - ptrStrStart) - 5;
                    }
                    if (ptrStr[-6] == value)
                    {
                        return (int)(ptrStr - ptrStrStart) - 6;
                    }
                    if (ptrStr[-7] == value)
                    {
                        return (int)(ptrStr - ptrStrStart) - 7;
                    }
                    ptrStr -= 8;
                }
                ptrStrEnd -= count & 7;
                while (ptrStr != ptrStrEnd)
                {
                    if (*ptrStr == value)
                    {
                        return (int)(ptrStr - ptrStrStart);
                    }
                    ptrStr--;
                }
                return -1;
            }
        }

        internal static unsafe int LastIndexOfAnyUnchecked(string str, string anyOf, int startIndex, int count)
        {

            if (anyOf.Length == 1)
            {
                return LastIndexOfUnchecked(str, anyOf[0], startIndex, count);
            }

            fixed (char* ptrStrStart = str)
            {
                fixed (char* ptrAnyOfStart = anyOf)
                {
                    char* ptrStr = ptrStrStart + startIndex;
                    char* ptrStrEnd = ptrStr - count;
                    char* ptrAnyOfEnd = ptrAnyOfStart + anyOf.Length;
                    while (ptrStr != ptrStrEnd)
                    {
                        for (char* ptrAnyOf = ptrAnyOfStart; ptrAnyOf != ptrAnyOfEnd; ptrAnyOf++)
                        {
                            if (*ptrAnyOf == *ptrStr)
                            {
                                return (int)(ptrStr - ptrStrStart);
                            }
                        }
                        ptrStr--;
                    }
                    return -1;
                }
            }
        }

        public static int IndexOf(string str, char value)
        {
            return IndexOfUnchecked(str, value, 0, str.Length);
        }

        public static int IndexOf(string str, string anyof)
        {
            return IndexOfAnyUnchecked(str, anyof, 0, str.Length);
        }

        public static int LastIndexOf(string str, char value)
        {
            return LastIndexOfUnchecked(str, value, str.Length - 1, str.Length);
        }

        public static int LastIndexOf(string str, string anyof)
        {
            return LastIndexOfAnyUnchecked(str, anyof, str.Length - 1, str.Length);
        }


        internal static bool IsDirectorySeparator(char c)
        {
            if (c != '\\')
            {
                return c == '/';
            }
            return true;
        }

        public unsafe static string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            int pos = path.Length;
            while (--pos >= 0)
            {
                if (IsDirectorySeparator(path[pos]))
                {
                    return path.Substring(pos + 1, path.Length - pos - 1);
                }
            }
            return path;
        }

        public unsafe static string GetFileNameWithoutExtension(string path)
        {
            string fileName = GetFileName(path);
            int pos = LastIndexOf(fileName, '.');
            if (pos != -1)
            {
                return fileName.Substring(0,pos);
            }
            return fileName;
        }

        /// <summary>
        /// 获取指定扩展名的文件路径
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="fileExtension"></param>
        /// <param name="files"></param>
        /// <returns>文件全路径列表</returns>
        public static int GetFiles(string searchDir, string fileExtension, bool recursive, ICollection<string> files)
        {
            return GetFilesNative(searchDir, fileExtension, recursive, files);
        }

        /// <summary>
        /// 获取指定扩展名的文件路径
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="fileExtension"></param>
        /// <param name="relativePath"></param>
        /// <param name="files"></param>
        /// <returns>文件相对路径列表</returns>
        public static int GetFiles(string searchDir, string fileExtension, string relativePath, bool recursive, ICollection<string> files)
        {
           return GetFilesNative(searchDir, fileExtension, relativePath, recursive, files);
        }

        /// <summary>
        /// 获取文件名包含提定内容的文件
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="searchPattern"></param>
        /// <param name="relativePath"></param>
        /// <param name="files"></param>
        /// <returns>文件相对路径列表</returns>
        public static int SearchFiles(string searchDir, string searchPattern, string relativePath, bool recursive, ICollection<string> files)
        {
            return SearchFilesNative(searchDir, searchPattern, relativePath, recursive, files);
        }

        /// <summary>
        /// 自定义文件过虑方法
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="filterFun"></param>
        /// <param name="relativePath"></param>
        /// <param name="files"></param>
        /// <returns>文件相对路径列表</returns>
        public static int SearchFiles(string searchDir, Func<string, string, bool> filterFun, string relativePath, bool recursive, ICollection<string> files)
        {
            return SearchFilesNative(searchDir, filterFun, relativePath, recursive, files);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="regexPattern"></param>
        /// <param name="relativePath"></param>
        /// <param name="files"></param>
        /// <returns>文件相对路径列表</returns>
        public static int SearchFilesRegex(string searchDir, string namePattern, string pathPattern, string relativePath, bool recursive, ICollection<string> files)
        {
            return SearchFilesRegexNative(searchDir, namePattern, pathPattern, relativePath, recursive, files);
        }

    }
}
