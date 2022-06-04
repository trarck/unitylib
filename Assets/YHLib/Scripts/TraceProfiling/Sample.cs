using System;
using System.Collections.Generic;

namespace YH.TraceProfiling
{
    [Serializable]
    internal class Sample
    {
        List<Sample> m_Children;
        List<Entry> m_Entries;

        public string Name { get; set; }
        public List<Sample> Children { get { if (m_Children == null) m_Children = new List<Sample>(); return m_Children; } }
        public List<Entry> Entries { get { if (m_Entries == null) m_Entries = new List<Entry>(); return m_Entries; } }
        public double DurationMS { get; private set; }
        public int ThreadId { get; set; }
        public double StartTime { get; set; }
        internal bool isThreaded;

        public bool HasChildren { get { return Children != null && Children.Count > 0; } }
        public bool HasEntries { get { return Entries != null && Entries.Count > 0; } }

        internal void Complete(double time)
        {
            DurationMS = time - StartTime;
        }
    }
}
