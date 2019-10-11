using System.Collections;
using System.Reflection;

namespace YH.Async
{
    public class Task
    {
        public int id;
        //if method is static target is null
        object m_Target;
        MethodInfo m_Method;
        TaskPool m_WorkPool;
        object[] m_Args;

        IEnumerator m_Enumerator;

        public Task(int id, TaskPool workPool, object target, MethodInfo method, params object[] args)
        {
            this.id = id;
            m_WorkPool = workPool;

            m_Target = target;
            m_Method = method;
            SetArgs(args);
        }

        public Task(int id, TaskPool workPool, IEnumerator enumerator)
        {
            this.id = id;
            m_WorkPool = workPool;

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
        }

        public void Done()
        {
            m_WorkPool.FinishTask(this);
        }

        public void Error()
        {
            m_WorkPool.ErrorTask(this);
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
