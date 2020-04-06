using System;
namespace YH.Log
{
    public class ConsoleTarget:ITarget
    {
        public ConsoleTarget()
        {
        }

        public void Init()
        {
        }

        public void Write(LogType type, string content)
        {
            Console.Write(content);
        }
        public void WriteLine(LogType type, string content)
        {
            Console.WriteLine(content);
        }
    }
}
