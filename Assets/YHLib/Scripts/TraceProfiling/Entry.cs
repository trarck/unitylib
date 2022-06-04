using System;

namespace YH.TraceProfiling
{
    [Serializable]
    internal struct Entry
    {
        public int ThreadId { get; set; }
        public double Time { get; set; }
        public string Message { get; set; }
    }
}
