using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;

namespace YH.TraceProfiling
{
    public struct MiniScopedSample : IDisposable
    {
        internal MiniScopedSample(string name,bool multiThreaded, string context)
        {
            MiniProfiler.BeginSample(name, multiThreaded,context);
        }

        /// <inheritdoc/>
        void IDisposable.Dispose()
        {
            MiniProfiler.EndSample();
        }
    }

    public static class MiniProfiler
    {
        static ThreadLocal<MiniProfilerImpl> m_ThreadedProfilers;

        static MiniProfilerImpl m_MainTraceProfiler;

        static MiniProfiler()
        {
            //create main thread profiler
            m_MainTraceProfiler = new MiniProfilerImpl();
        }
        internal static MiniProfilerImpl GetThreadSafeProfiler()
        {
            if (m_ThreadedProfilers != null)
            {
                if (!m_ThreadedProfilers.IsValueCreated)
                {
                    m_ThreadedProfilers.Value = new MiniProfilerImpl(true);
                }
                return m_ThreadedProfilers.Value;
            }
            return m_MainTraceProfiler;
        }

        [Conditional("YH_PROFILE")]
        public static void BeginSample(string name, bool multiThreaded, string content = null)
        {
            MiniProfilerImpl profiler = GetThreadSafeProfiler();
            profiler.BeginSample(name, multiThreaded);
            if (!string.IsNullOrEmpty(content))
            {
                profiler.AddEntry(content);
            }
        }

        [Conditional("YH_PROFILE")]
        public static void EndSample()
        {
            MiniProfilerImpl profiler = GetThreadSafeProfiler();
            profiler.EndSample();
        }

        public static MiniScopedSample ScopedSample(string name, bool multiThreaded = false)
        {
            return new MiniScopedSample(name, multiThreaded, null);
        }

        public static MiniScopedSample ScopedSample(string name, string context)
        {
            return new MiniScopedSample(name, false, context);
        }

        [Conditional("YH_PROFILE")]
        public static void Save(string outputFile)
        {
            MiniProfilerImpl profiler = GetThreadSafeProfiler();
            System.IO.File.WriteAllText(outputFile, profiler.FormatForTraceEventProfiler());
        }

        public static string FormatForTraceEventProfiler()
        {
            MiniProfilerImpl profiler = GetThreadSafeProfiler();
            return profiler.FormatForTraceEventProfiler();
        }

        internal static void BeginThreadProfiling()
        {
            Debug.Assert(m_ThreadedProfilers == null);
            m_ThreadedProfilers = new ThreadLocal<MiniProfilerImpl>(true);
            m_ThreadedProfilers.Value = m_MainTraceProfiler;
        }

        internal static void EndThreadProfiling(Sample node)
        {
            foreach (MiniProfilerImpl subLog in m_ThreadedProfilers.Values)
            {
                if (subLog != m_MainTraceProfiler)
                {
                    OffsetTimesR(subLog.Root, node.StartTime);
                    if (subLog.Root.HasChildren)
                        node.Children.AddRange(subLog.Root.Children);

                    if (subLog.Root.HasEntries)
                        node.Entries.AddRange(subLog.Root.Entries);
                }
            }
            m_ThreadedProfilers.Dispose();
            m_ThreadedProfilers = null;
        }
        private static void OffsetTimesR(Sample step, double offset)
        {
            step.StartTime += offset;
            if (step.HasEntries)
            {
                for (int i = 0; i < step.Entries.Count; i++)
                {
                    Entry e = step.Entries[i];
                    e.Time = e.Time + offset;
                    step.Entries[i] = e;
                }
            }
            if (step.HasChildren)
                foreach (Sample subStep in step.Children)
                    OffsetTimesR(subStep, offset);
        }
    }

    internal class MiniProfilerImpl
    {
        Sample m_Root;
        [NonSerialized]
        Stack<Sample> m_Stack;

        [NonSerialized]
        Stopwatch m_WallTimer;
        private List<Tuple<string, string>> m_MetaData = new List<Tuple<string, string>>();

        internal Sample Root
        {
            get { return m_Root; }
        }

        void Init(bool onThread)
        {
            m_WallTimer = Stopwatch.StartNew();
            m_Root = new Sample();
            m_Stack = new Stack<Sample>();
            m_Stack.Push(m_Root);

            AddMetaData("Date", DateTime.Now.ToString());
        }

        public MiniProfilerImpl()
        {
            Init(false);
        }

        internal MiniProfilerImpl(bool onThread)
        {
            Init(onThread);
        }

        public void AddMetaData(string key, string value)
        {
            m_MetaData.Add(new Tuple<string, string>(key, value));
        }

        public void BeginSample(string name, bool multiThreaded)
        {
            Sample node = new Sample();
            node.Name = name;
            node.StartTime = m_WallTimer.Elapsed.TotalMilliseconds;
            node.ThreadId = Thread.CurrentThread.ManagedThreadId;
            node.isThreaded = multiThreaded;
            m_Stack.Peek().Children.Add(node);
            m_Stack.Push(node);
            if (multiThreaded)
            {
                MiniProfiler.BeginThreadProfiling();
            }
        }

        public void EndSample()
        {
            Debug.Assert(m_Stack.Count > 1);
            Sample node = m_Stack.Pop();
            node.Complete(m_WallTimer.Elapsed.TotalMilliseconds);

            if (node.isThreaded)
            {
                MiniProfiler.EndThreadProfiling(node);
            }
        }
        public void AddEntry(string msg)
        {
            Entry entry = new Entry() { Message = msg, Time = m_WallTimer.Elapsed.TotalMilliseconds, ThreadId = Thread.CurrentThread.ManagedThreadId };
            m_Stack.Peek().Entries.Add(entry);
        }

        static void AppendLineIndented(StringBuilder builder, int indentCount, string text)
        {
            for (int i = 0; i < indentCount; i++)
                builder.Append(" ");
            builder.AppendLine(text);
        }

        static void PrintNodeR(bool includeSelf, StringBuilder builder, int indentCount, Sample node)
        {
            if (includeSelf)
                AppendLineIndented(builder, indentCount, $"[{node.Name}] {node.DurationMS * 1000}us");
            foreach (var msg in node.Entries)
            {
                string line = msg.Message;
                AppendLineIndented(builder, indentCount + 1, line);
            }
            foreach (var child in node.Children)
                PrintNodeR(true, builder, indentCount + 1, child);
        }

        internal string FormatAsText()
        {
            using (new CultureScope())
            {
                StringBuilder builder = new StringBuilder();
                PrintNodeR(false, builder, -1, Root);
                return builder.ToString();
            }
        }

        static string CleanJSONText(string message)
        {
            return message.Replace("\\", "\\\\");
        }

        static IEnumerable<string> IterateTEPLines(bool includeSelf, Sample node)
        {
            ulong us = (ulong)(node.StartTime * 1000);

            string argText = string.Empty;
            if (node.Entries.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(", \"args\": {");
                for (int i = 0; i < node.Entries.Count; i++)
                {
                    string line = node.Entries[i].Message;
                    builder.Append($"\"{i}\":\"{CleanJSONText(line)}\"");
                    if (i < (node.Entries.Count - 1))
                        builder.Append(", ");
                }
                builder.Append("}");
                argText = builder.ToString();
            }

            if (includeSelf)
                yield return "{" + $"\"name\": \"{CleanJSONText(node.Name)}\", \"ph\": \"X\", \"dur\": {node.DurationMS * 1000}, \"tid\": {node.ThreadId}, \"ts\": {us}, \"pid\": 1" + argText + "}";

            foreach (var child in node.Children)
                foreach (var r in IterateTEPLines(true, child))
                    yield return r;
        }

        class CultureScope : IDisposable
        {
            CultureInfo m_Prev;
            public CultureScope()
            {
                m_Prev = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            }

            public void Dispose()
            {
                Thread.CurrentThread.CurrentCulture = m_Prev;
            }
        }


        public string FormatForTraceEventProfiler()
        {
            using (new CultureScope())
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("{");

                foreach (Tuple<string, string> tuple in m_MetaData)
                    builder.AppendLine($"\"{tuple.Item1}\": \"{tuple.Item2}\",");

                builder.AppendLine("\"traceEvents\": [");
                int i = 0;
                foreach (string line in IterateTEPLines(false, Root))
                {
                    if (i != 0)
                        builder.Append(",");
                    builder.AppendLine(line);
                    i++;
                }
                builder.AppendLine("]");
                builder.AppendLine("}");
                return builder.ToString();
            }
        }
    }
}
