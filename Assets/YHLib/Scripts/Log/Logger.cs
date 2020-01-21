using System;
using System.Collections.Generic;
using System.IO;
namespace YH.Log
{
    public class Logger : ILogger
    {
        List<ITarget> m_Targets;
        LogType m_LogLevel= LogType.Debug;

        public Logger()
        {
            
        }

        public Logger(LogType logLevel)
        {
            m_LogLevel = logLevel;
        }

        #region Log

        private void Log(LogType type, string content)
        {
            //content = string.Format("{0} [{1}]:{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss "), type, content);
            if (m_Targets != null)
            {
                foreach (var target in m_Targets)
                {
                    target.WriteLine(type,content);
                }
            }
        }

        public void Debug(string content)
        {
            if (IsLogTypeEnable(LogType.Debug))
            {
                Log(LogType.Debug, content);
            }
        }

        public void Info(string content)
        {
            if (IsLogTypeEnable(LogType.Info))
            {
                Log(LogType.Info, content);
            }
        }

        public void Warn(string content)
        {
            if (IsLogTypeEnable(LogType.Warning))
            {
                Log(LogType.Warning, content);
            }
        }

        public void Error(string content)
        {
            if (IsLogTypeEnable(LogType.Error))
            {
                Log(LogType.Error, content);
            }
        }

        public void Fatal(string content)
        {
            if (IsLogTypeEnable(LogType.Fatal))
            {
                Log(LogType.Fatal, content);
            }
        }

        protected bool IsLogTypeEnable(LogType type)
        {
            return type >= m_LogLevel;
        }
        public LogType logLevel
        {
            get
            {
                return m_LogLevel;
            }
            set
            {
                m_LogLevel = value;
            }
        }
        #endregion

        #region Target
        public void AddTarget(ITarget target)
        {
            targets.Add(target);
        }

        public void RemoveTarget(ITarget target)
        {
            targets.Remove(target);
        }

        public List<ITarget> targets
        {
            get
            {
                if (m_Targets == null)
                {
                    m_Targets = new List<ITarget>();
                }
                return m_Targets;
            }
            set
            {
                m_Targets = value;
            }
        }
        #endregion
    }
}
