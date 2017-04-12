using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace YH
{
    public abstract class FileFilterRule
    {
        public abstract bool Check(string filepath);
    }

    public class FileSizeRule: FileFilterRule
    {
        protected long m_From = -1;
        protected long m_End = -1;

        public FileSizeRule(long from,long end=-1)
        {
            m_From = from;
            m_End = end;
        }

        public override bool Check(string filepath)
        {
            FileInfo info = new FileInfo(filepath);

            return Check(info.Length);
        }

        public bool Check(long size)
        {
            if(m_From>-1 && size < m_From)
            {
                return false;
            }

            if(m_End>-1 && size>m_End)
            {
                return false;
            }
            return true;
        }
    }

    public class FileNameRule: FileFilterRule
    {
        protected Regex m_Regex;
        FileNameRule(Regex regex)
        {
            m_Regex = regex;
        }

        public override bool Check(string filepath)
        {
            return m_Regex.IsMatch(filepath);
        }
    }

    public class FileExtRule: FileFilterRule
    {
        protected List<string> m_Exts;

        public FileExtRule(List<string> exts)
        {
            m_Exts = exts;
        }

        public FileExtRule(string exts)
        {
            if (string.IsNullOrEmpty(exts))
            {
                string[] items = exts.Split(';');
                m_Exts = new List<string>(items);
            }
        }

        public override bool Check(string filepath)
        {
            if(m_Exts!=null)
            {
                string fileExt = Path.GetExtension(filepath);
                foreach (string ext in m_Exts)
                {
                    if (ext == fileExt)
                    {
                        return true;
                    }
                }
            }
           

            return false;
        }
    }

    public class FileDirRule : FileFilterRule
    {
        protected List<string> m_Dirs;

        public FileDirRule(List<string> dirs)
        {
            m_Dirs = dirs;
        }

        public FileDirRule(string dirs)
        {
            if(string.IsNullOrEmpty(dirs))
            {
                string[] items = dirs.Split(';');
                m_Dirs = new List<string>(items);
            }
        }

        public override bool Check(string filepath)
        {
            if (m_Dirs!=null)
            {
                string fileDir = Path.GetDirectoryName(filepath);
                foreach (string dir in m_Dirs)
                {
                    if (fileDir.IndexOf(dir) > -1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}