﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace YH.Async
{
    public class ThreadWorkPool
    {
        static int TaskId = 0;
        const int DefaultLimit = 4;

        public const int UnknowError = -1;
        private int m_Limit;
        private Queue<Task> m_PendingTasks = null;
        private Dictionary<int, Task> m_RunningTasks = null;
        private bool m_Running = true;
        private bool m_Finished = false;
        private bool m_Completed = false;
        private bool m_StopOnError = false;
        private bool m_Joined = false;
        private int m_ErrorCode = 0;
        private List<Task> m_ErrorTasks = null;

        public event Action<int> onComplete;

        public List<Task> errorTasks
        {
            get
            {
                return m_ErrorTasks;
            }
        }

        public int limit
        {
            get
            {
                return m_Limit;
            }
            set
            {
                m_Limit = value;
            }
        }

        public ThreadWorkPool() : this(DefaultLimit)
        {

        }

        public ThreadWorkPool(int limit)
        {
            m_Limit = limit;
            m_PendingTasks = new Queue<Task>();
            m_RunningTasks = new Dictionary<int, Task>();
            m_ErrorTasks = new List<Task>();
        }

        protected void Add(object target, MethodInfo method, params object[] args)
        {
            if (!m_Running)
            {
                return;
            }

            Task task = new Task(++TaskId, this, target, method, args);

            if (m_RunningTasks.Count < m_Limit)
            {
                m_RunningTasks[task.id] = task;
                task.Run();
            }
            else
            {
                m_PendingTasks.Enqueue(task);
            }
        }

        #region Action
        public delegate void ActionHandle(Task t);
        public delegate void ActionHandle<T1>(Task t, T1 arg1);
        public delegate void ActionHandle<T1, T2>(Task t, T1 arg1, T2 arg2);
        public delegate void ActionHandle<T1, T2, T3>(Task t, T1 arg1, T2 arg2, T3 arg3);
        public delegate void ActionHandle<T1, T2, T3, T4>(Task t, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        public delegate void ActionHandle<in T1, in T2, in T3, in T4, in T5>(Task t, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public delegate void ActionHandle<in T1, in T2, in T3, in T4, in T5, in T6>(Task t, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        public void Add(ActionHandle action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T>(ActionHandle<T> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2>(ActionHandle<T1, T2> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3>(ActionHandle<T1, T2, T3> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3, T4>(ActionHandle<T1, T2, T3, T4> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3, T4, T5>(ActionHandle<T1, T2, T3, T4, T5> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3, T4, T5, T6>(ActionHandle<T1, T2, T3, T4, T5, T6> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }
        #endregion

        #region Coroutine Action
        public delegate UnityEngine.Coroutine CoroutineHandle(IEnumerator enumerator);
        public CoroutineHandle coroutineHandle;
        public delegate IEnumerator EnumeratorAction(Task t);
        public delegate IEnumerator EnumeratorAction<T>(Task t, T arg);
        public delegate IEnumerator EnumeratorAction<T1, T2>(Task t, T1 arg1, T2 arg2);
        public delegate IEnumerator EnumeratorAction<T1, T2, T3>(Task t, T1 arg1, T2 arg2, T3 arg3);
        public delegate IEnumerator EnumeratorAction<T1, T2, T3, T4>(Task t, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        public delegate IEnumerator EnumeratorAction<T1, T2, T3, T4, T5>(Task t, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public delegate IEnumerator EnumeratorAction<T1, T2, T3, T4, T5, T6>(Task t, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

        public void Add(EnumeratorAction action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T>(EnumeratorAction<T> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2>(EnumeratorAction<T1, T2> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3>(EnumeratorAction<T1, T2, T3> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3, T4>(EnumeratorAction<T1, T2, T3, T4> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3, T4, T5>(EnumeratorAction<T1, T2, T3, T4, T5> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3, T4, T5, T6>(EnumeratorAction<T1, T2, T3, T4, T5, T6> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }
        #endregion

        public virtual UnityEngine.Coroutine StartCoroutine(IEnumerator routine)
        {
            if (coroutineHandle != null)
            {
                coroutineHandle(routine);
            }
            return null;
        }

        public void FinishTask(Task task)
        {
            m_RunningTasks.Remove(task.id);
            Continue();
        }

        public void ErrorTask(Task task, int errorCode = UnknowError)
        {
            m_ErrorTasks.Add(task);

            m_RunningTasks.Remove(task.id);
            if (m_StopOnError || task.breakOnError)
            {
                m_ErrorCode = errorCode;
                Finish();
            }
            else
            {
                Continue();
            }
        }

        /// <summary>
        /// 所有task都添加后调用
        /// </summary>
        public void Join()
        {
            m_Joined = true;
            if (m_Finished)
            {
                Complete();
            }
        }

        protected void Continue()
        {
            //check is finish
            if (m_RunningTasks.Count == 0 && m_PendingTasks.Count == 0)
            {
                Finish();
            }

            //check is runable.
            if (!m_Running)
            {
                return;
            }

            if (m_PendingTasks.Count > 0 && m_RunningTasks.Count < m_Limit)
            {
                int emptyCount = m_Limit - m_RunningTasks.Count;
                emptyCount = emptyCount > m_PendingTasks.Count ? m_PendingTasks.Count : emptyCount;
                for (int i = 0; i < emptyCount; ++i)
                {
                    Task task = m_PendingTasks.Dequeue();
                    m_RunningTasks[task.id] = task;
                    task.Run();
                }
            }
        }

        protected void Finish()
        {
            m_Finished = true;
            m_Running = false;

            if (m_Joined)
            {
                Complete();
            }
        }

        protected virtual void Complete()
        {
            m_Completed = true;
            TriggerCompleteEvent();
        }

        protected void TriggerCompleteEvent()
        {
            if (onComplete != null)
            {
                onComplete(m_ErrorCode);
            }
        }

        public void Clear()
        {
            m_Limit = DefaultLimit;
            if (m_PendingTasks != null)
            {
                m_PendingTasks.Clear();
            }

            if (m_PendingTasks != null)
            {
                m_PendingTasks.Clear();
            }

            if (m_ErrorTasks != null)
            {
                m_ErrorTasks.Clear();
            }

            m_Running = true;
            m_Finished = false;
            m_Completed = false;
            m_StopOnError = false;
            m_Joined = false;
            m_ErrorCode = 0;
            onComplete = null;
        }

        public bool stopOnError
        {
            get
            {
                return m_StopOnError;
            }
            set
            {
                m_StopOnError = value;
            }
        }

        public bool completed
        {
            get
            {
                return m_Completed;
            }
            set
            {
                m_Completed = value;
            }
        }
    }

    public class WorkPoolFactory
    {
        private static readonly Stack<WorkPool> m_Stack = new Stack<WorkPool>();

        class WorkPoolEx : WorkPool
        {
            public WorkPoolEx()
            {

            }

            public WorkPoolEx(int limit) : base(limit)
            {

            }

            protected override void Complete()
            {
                base.Complete();
                WorkPoolFactory.Release(this);
            }
        }

        public static WorkPool Get()
        {
            WorkPool element;
            if (m_Stack.Count == 0)
            {
                element = new WorkPoolEx();
            }
            else
            {
                element = m_Stack.Pop();
            }
            return element;
        }

        public static WorkPool Get(int limit)
        {
            WorkPool element;
            if (m_Stack.Count == 0)
            {
                element = new WorkPoolEx(limit);
            }
            else
            {
                element = m_Stack.Pop();
            }
            return element;
        }

        public static void Release(WorkPool element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            element.Clear();
            m_Stack.Push(element);
        }
    }
}
