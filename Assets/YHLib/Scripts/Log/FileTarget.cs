using System;
using System.IO;
using System.Text;

namespace YH.Log
{
    public class FileTarget:ITarget,IDisposable
    {
        string m_FilePath;
        StreamWriter m_Stream;

        public FileTarget(string filePath)
        {
            m_FilePath = filePath;
        }

        public void Init()
        {
            if (m_Stream != null)
            {
                return;
            }

            if (!File.Exists(m_FilePath))
            {
                string dir = Path.GetDirectoryName(m_FilePath);
                if (!Directory.Exists(dir))
                {
                    DirectoryInfo dirInfo= Directory.CreateDirectory(dir);
                    if (!dirInfo.Exists)
                    {
                        return;
                    }
                }
            }

            m_Stream = new StreamWriter(m_FilePath, true, Encoding.Default);
            m_Stream.AutoFlush = true;
        }

        public void Write(LogType type, string content)
        {
            if (m_Stream != null)
            {
                m_Stream.WriteLineAsync(content);
            }
        }

        public void WriteLine(LogType type, string content)
        {
            if (m_Stream != null)
            {
                m_Stream.WriteLineAsync(content);
                m_Stream.WriteAsync("\n");
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    m_FilePath = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                m_Stream.Close();
                m_Stream = null;

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~FileTarget()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion

        public static string GetDefultFilePath(string dirName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory + dirName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            }
            return path;
        }
    }
}
