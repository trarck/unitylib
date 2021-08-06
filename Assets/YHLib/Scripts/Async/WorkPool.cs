using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace YH.Async
{
    public class WorkPool
    {
        public enum State
        {
            Idle,
            Running,
            Finish,
            Complete
        }

        public static int TaskId = 0;
        public const int DefaultLimit = 4;

        public const int UnknowError = -1;
        private int m_Limit;
        private Queue<ITask> m_PendingTasks = null;
        //key:task id
        private Dictionary<int, ITask> m_RunningTasks = null;
        private List<ITask> m_ErrorTasks = null;

        private State m_State;
        private bool m_Running = true;
        private bool m_Finished = false;
        private bool m_Completed = false;
        private bool m_StopOnError = false;
        private bool m_Joined = false;
        private int m_ErrorCode = 0;


        public event Action<int> onComplete;

        public List<ITask> errorTasks
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

        private bool canAddTask
        {
			get
            {
                return  m_State==State.Idle || m_State == State.Running || (m_State == State.Finish && !m_Joined);
            }
        }

        public WorkPool() : this(DefaultLimit)
        {

        }

        public WorkPool(int limit)
        {
            m_Limit = limit;
            m_State = State.Idle;
            m_PendingTasks = new Queue<ITask>();
            m_RunningTasks = new Dictionary<int, ITask>();
            m_ErrorTasks = new List<ITask>();
        }

        protected void Add(ITask task)
        {
            if (!canAddTask)
            {
                Debug.LogError("[WorkPool] can't add task!");
                return;
            }

            if (task.id == 0)
            {
                task.id = ++TaskId;
            }

            if (m_State == State.Running || m_State==State.Finish)
            {
                if (m_State == State.Finish)
                {
                    m_State = State.Running;
                }

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
            else
            {
                m_PendingTasks.Enqueue(task);
            }
        }
     
        public void DoTaskDone(ITask task)
        {
            m_RunningTasks.Remove(task.id);
            Next();
        }

        public void DoTaskError(ITask task, int errorCode = UnknowError)
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
                Next();
            }
        }

        public void Start()
        {
            m_State = State.Running;
            Next();
        }


        public void Next()
        {
            //check is finish
            if (m_RunningTasks.Count == 0 && m_PendingTasks.Count == 0)
            {
                Finish();
            }

            //check is runable.
            if (m_State!=State.Running)
            {
                return;
            }

            if (m_PendingTasks.Count > 0 && m_RunningTasks.Count < m_Limit)
            {
                int emptyCount = m_Limit - m_RunningTasks.Count;
                emptyCount = emptyCount > m_PendingTasks.Count ? m_PendingTasks.Count : emptyCount;
                for (int i = 0; i < emptyCount; ++i)
                {
                    ITask task = m_PendingTasks.Dequeue();
                    m_RunningTasks[task.id] = task;
                    task.Run();
                }
            }
        }

        public void Finish()
        {
            m_State = State.Finish;

            if (m_Joined)
            {
                Complete();
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

        protected virtual void Complete()
        {
            m_State = State.Complete;
            TriggerCompleteEvent();
        }

        protected void TriggerCompleteEvent()
        {
            if (onComplete != null)
            {
                onComplete(m_ErrorCode);
            }
        }

        public virtual void Clean()
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
    }
}
