using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YH.Async
{
    public class TaskPool
    {
        static int TaskId=0;

        string m_Name;
        int m_Limit;
        Queue<Task> m_PendingTasks = null;
        Dictionary<int,Task> m_RunningTasks = null;
        bool m_Running = true;
        bool m_Completed = false;
        bool m_StopOnError = false;
        bool m_Joined = false;
        bool m_HaveError = false;

        public event Action<bool> onComplete;

        public TaskPool(string name,int limit)
        {
            m_Name = name;
            m_Limit = limit;
            m_PendingTasks = new Queue<Task>();
            m_RunningTasks = new Dictionary<int, Task>();
        }


        void Add(object target, MethodInfo method, params object[] args)
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
        public void Add(Action<Task> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T>(Action<Task,T> action,params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1,T2>(Action<Task, T1,T2> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1,T2,T3>(Action<Task, T1,T2,T3> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3,T4>(Action<Task, T1, T2, T3,T4> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3, T4,T5>(Action<Task, T1, T2, T3, T4,T5> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3, T4, T5,T6>(Action<Task, T1, T2, T3, T4, T5, T6> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }
        #endregion

        #region Coroutine Action
        public delegate UnityEngine.Coroutine CoroutineHandle(IEnumerator enumerator);
        public CoroutineHandle StartCoroutine;
        public delegate IEnumerator EnumeratorAction(Task t);
        public delegate IEnumerator EnumeratorAction<T>(Task t,T arg);
        public delegate IEnumerator EnumeratorAction<T1,T2>(Task t, T1 arg1,T2 arg2);
        public delegate IEnumerator EnumeratorAction<T1,T2,T3>(Task t, T1 arg1, T2 arg2,T3 arg3);
        public delegate IEnumerator EnumeratorAction<T1, T2, T3,T4>(Task t, T1 arg1, T2 arg2, T3 arg3,T4 arg4);
        public delegate IEnumerator EnumeratorAction<T1, T2, T3, T4,T5>(Task t, T1 arg1, T2 arg2, T3 arg3, T4 arg4,T5 arg5);
        public delegate IEnumerator EnumeratorAction<T1, T2, T3, T4, T5,T6>(Task t, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,T6 arg6);

        public void Add(EnumeratorAction action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T>(EnumeratorAction<T> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1,T2>(EnumeratorAction<T1,T2> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2,T3>(EnumeratorAction<T1, T2,T3> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3,T4>(EnumeratorAction<T1, T2, T3,T4> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3, T4,T5>(EnumeratorAction<T1, T2, T3, T4,T5> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        public void Add<T1, T2, T3, T4, T5,T6>(EnumeratorAction<T1, T2, T3, T4, T5,T6> action, params object[] args)
        {
            Add(action.Target, action.Method, args);
        }

        #endregion

        public void FinishTask(Task task)
        {
            m_RunningTasks.Remove(task.id);
            Continue();
        }

        public void ErrorTask(Task task)
        {
            m_RunningTasks.Remove(task.id);
            if (m_StopOnError)
            {
                m_HaveError = true;
                Complete();
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
            if (m_Completed)
            {
                TriggerCompleteEvent();
            }
        }

        void Continue()
        {
            //check is finish
            if (m_RunningTasks.Count == 0 && m_PendingTasks.Count == 0)
            {
                Complete();
            }

            //check is runable.
            if (!m_Running)
            {
                return;
            }

            if(m_PendingTasks.Count>0 && m_RunningTasks.Count < m_Limit)
            {
                int emptyCount = m_Limit - m_RunningTasks.Count;
                emptyCount = emptyCount > m_PendingTasks.Count ? m_PendingTasks.Count : emptyCount;
                for(int i = 0; i < emptyCount; ++i)
                {
                    Task task = m_PendingTasks.Dequeue();
                    m_RunningTasks[task.id] = task;
                    task.Run();
                }
            }
        }

        void Complete()
        {
            m_Completed = true;
            m_Running = false;

            if (m_Joined)
            {
                TriggerCompleteEvent();
            }
        }

        void TriggerCompleteEvent()
        {
            if (onComplete != null)
            {
                onComplete(!m_HaveError);
            }
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
}
