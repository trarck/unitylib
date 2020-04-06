using System.Collections.Generic;

namespace YH.Log
{
    public class LoggerFactory
    {
        static ILogger s_DefaultLogger;
        static Dictionary<string, ILogger> s_Loggers=new Dictionary<string, ILogger>();

        public static ILogger GetLogger(string name)
        {
            ILogger logger = null;
            if(!s_Loggers.TryGetValue(name,out logger))
            {
                logger = new Logger();
            }
            return logger;
        }

        public static ILogger GetDefaultLogger()
        {
            if (s_DefaultLogger == null)
            {
                s_DefaultLogger = new Logger();
            }
            return s_DefaultLogger;
        }

        public static ILogger GetFileLogger(string file)
        {
            Logger logger = new Logger();
            ITarget target = TargetFactory.GetTarget(TargetType.File, file);
            logger.AddTarget(target);
            return logger;
        }
    }
}
