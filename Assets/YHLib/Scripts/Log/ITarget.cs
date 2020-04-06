namespace YH.Log
{
    public interface ITarget
    {
        void Init();

        void Write(LogType type, string content);
        void WriteLine(LogType type, string content);
    }
}
