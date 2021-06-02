using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YH.Async
{
    public class Task
    {
        public int id;
        public bool breakOnError = false;
        //if method is static target is null
        private object m_Target;
        private MethodInfo m_Method;
        private WorkPool m_WorkPool;
        private object[] m_Args;
        private IEnumerator m_Enumerator;

        public Task(int id, WorkPool workPool, object target, MethodInfo method, params object[] args)
        {
            this.id = id;
            m_WorkPool = workPool;

            m_Target = target;
            m_Method = method;
            SetArgs(args);
        }

        public Task(int id, WorkPool workPool, IEnumerator enumerator)
        {
            this.id = id;
            m_WorkPool = workPool;
            m_Enumerator = enumerator;
        }

        public void Run()
        {
            if (m_Method!=null)
            {
                IEnumerator ret = m_Method.Invoke(m_Target, m_Args) as IEnumerator;
                if (ret != null && m_WorkPool!=null)
                {
                    m_WorkPool.StartCoroutine(ret);
                }
            }
            else if (m_Enumerator != null)
            {
                m_WorkPool.StartCoroutine(m_Enumerator);
            }
        }

        public void Done()
        {
            m_WorkPool.FinishTask(this);
        }

        public void Error(int errorCode)
        {
            m_WorkPool.ErrorTask(this, errorCode);
        }

        public void Clear()
        {
            id = 0;
            m_Target = null;
            m_Method = null;
            m_WorkPool = null;
            m_Args = null;

            m_Enumerator = null;
        }

        void SetArgs(object[] args)
        {
            m_Args = new object[args.Length + 1];

            m_Args[0] = this;
            for (int i = 0; i < args.Length; ++i)
            {
                m_Args[i + 1] = args[i];
            }
        }
    }
}
