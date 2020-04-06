using System.Collections.Generic;
using UnityEngine;
using YH.Pool;

namespace YH.Async
{
    public class ActionGroupPool
    {
        private static readonly Stack<ActionGroup> m_Stack = new Stack<ActionGroup>();

        public class ActionGroupEx : ActionGroup
        {
            protected override void DoComplete()
            {
                base.DoComplete();
                ActionGroupPool.Release(this);
            }
        }

        public static ActionGroup Get()
        {
            ActionGroup element;
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

        public static void Release(ActionGroup element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            element.Clean();
            m_Stack.Push(element);
        }
    }
}
