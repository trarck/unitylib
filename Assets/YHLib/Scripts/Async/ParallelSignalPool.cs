using System.Collections.Generic;
using UnityEngine;
using YH.Pool;

namespace YH.Async
{
    public class ParallelSignalPool
    {
        private static readonly Stack<ParallelSignal> m_Stack = new Stack<ParallelSignal>();

        public class ActionGroupEx : ParallelSignal
        {
            protected override void DoComplete()
            {
                base.DoComplete();
                ParallelSignalPool.Release(this);
            }
        }

        public static ParallelSignal Get()
        {
            ParallelSignal element;
            if (m_Stack.Count == 0)
            {
                element = new ActionGroupEx();
            }
            else
            {
                element = m_Stack.Pop();
            }
            return element;
        }

        public static void Release(ParallelSignal element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            element.Clean();
            m_Stack.Push(element);
        }
    }
}
