using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YH.AssetManager
{
    internal class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly UnityAction<T> m_ActionOnGet;
        private readonly UnityAction<T> m_ActionOnRelease;
        private readonly UnityAction<T> m_ActionOnDispose;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease, UnityAction<T> actionOnDispose)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
            m_ActionOnDispose = actionOnDispose;
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);
            m_Stack.Push(element);
        }

        public void Dispose()
        {
            foreach (T t in m_Stack)
            {
                if (m_ActionOnDispose != null)
                {
                    m_ActionOnDispose(t);
                }
                else if (t is System.IDisposable)
                {
                    (t as System.IDisposable).Dispose();
                }
            }
        }
    }
}
