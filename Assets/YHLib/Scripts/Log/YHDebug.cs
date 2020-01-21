using UnityEngine;

namespace YH.Log
{
    public class YHDebug
    {
        [System.Diagnostics.Conditional("YHLOG_ON")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        [System.Diagnostics.Conditional("YHLOG_ON")]
        public static void LogFormat(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }

        [System.Diagnostics.Conditional("YHLOG_ON")]
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        [System.Diagnostics.Conditional("YHLOG_ON")]
        public static void LogWarningFormat(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }

        [System.Diagnostics.Conditional("YHLOG_ERROR_ON")]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        [System.Diagnostics.Conditional("YHLOG_ERROR_ON")]
        public static void LogErrorFormat(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }

        [System.Diagnostics.Conditional("YHLOG_ERROR_ON")]
        public static void LogException(System.Exception exception, Object context)
        {
            Debug.LogException(exception, context);
        }

        [System.Diagnostics.Conditional("YHLOG_ERROR_ON")]
        public static void LogException(System.Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}