using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace YH.AM
{
    public class AssetBinaryDiffRule
    {
        long m_PatchMinFileSize = 128;
        List<string> m_PatchBlackDirs;
        List<string> m_PatchBlackFileExts;

        FileSizeRule m_FileSizeRule;
        FileDirRule m_BlackDirRule;
        FileExtRule m_FileExtRule;

        public bool Check(string filepath)
        {
            //文件要大小设置值才进行差分
            if(m_FileSizeRule!=null && !m_FileSizeRule.Check(filepath))
            {
                return false;
            }

            //文件不在黑名单中，进行差分
            if(m_BlackDirRule!=null && m_BlackDirRule.Check(filepath))
            {
                return false;
            }

            //文件的扩展名不在列表中，进行差分
            if(m_FileExtRule!=null && m_FileExtRule.Check(filepath))
            {
                return false;
            }

            return true;
        }

        public long PatchMinFileSize
        {
            set
            {
                m_PatchMinFileSize = value;
                m_FileSizeRule = new FileSizeRule(m_PatchMinFileSize);
            }

            get
            {
                return m_PatchMinFileSize;
            }
        }

        public List<string> PatchBlackDirs
        {
            set
            {
                m_PatchBlackDirs = value;
                m_BlackDirRule = new FileDirRule(m_PatchBlackDirs);
            }

            get
            {
                return m_PatchBlackDirs;
            }
        }

        public List<string> PatchBlackFileExts
        {
            set
            {
                m_PatchBlackFileExts = value;
                m_FileExtRule = new FileExtRule(m_PatchBlackFileExts);
            }

            get
            {
                return m_PatchBlackFileExts;
            }
        }
    }
}