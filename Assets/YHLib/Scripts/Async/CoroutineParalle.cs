using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace YH.Async
{

	public class CoroutineTask : ITask
	{
        private CoroutineParalle m_Owner;
        private IEnumerator m_Enumerator;

        public int id {get;set;}
		public bool breakOnError {get; set;}

        public CoroutineTask(CoroutineParalle onwer,IEnumerator enumerator)
        {
            m_Owner = onwer;
            m_Enumerator = enumerator;
        }

        public void Clean()
		{
            m_Owner = null;
            m_Enumerator = null;
		}

		public void Done()
		{
            m_Owner.DoTaskDone(this);
		}

		public void Error(int errorCode)
		{
            m_Owner.DoTaskError(this, errorCode);
		}

		public void Run()
		{
            m_Owner.StartCoroutine(m_Enumerator);
        }
	}

	public class CoroutineParalle:WorkPool
    {

        public delegate Coroutine CoroutineHandle(IEnumerator enumerator);
        public CoroutineHandle coroutineHandle;

        public CoroutineParalle() : base(DefaultLimit)
        {

        }

        public CoroutineParalle(int limit) :base(limit)
        {

        }

        public virtual Coroutine StartCoroutine(IEnumerator routine)
        {
            if (coroutineHandle != null)
            {
                coroutineHandle(routine);
            }
            return null;
        }

		public override void Clean()
		{
            coroutineHandle = null;
            base.Clean();
		}
	}
}
