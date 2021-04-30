using System;
using System.Collections.Generic;
using System.Text;

namespace YH.Async
{
    public class ParallelSignal
    {
        public Action completeHandle;

        int m_TaskCount=0;
        int m_FinishedCount = 0;
        bool m_Joined = false;

        public ParallelSignal()
        {
            m_TaskCount = 0;
        }

        public ParallelSignal(int total)
        {
            m_TaskCount = total;
        }

        public void DoActionFinish()
        {
            ++m_FinishedCount;
            //check finished
            CheckFinished();
        }

        public void Increase()
        {
            ++m_TaskCount;
        }

        public void Increase(int count)
        {
            m_TaskCount+=count;
        }

        public void Decrease()
        {
            --m_TaskCount;
        }

        public void Decrease(int count)
        {
            m_TaskCount-=count;
        }

        public void Join()
        {
            m_Joined = true;
            //check finished
            CheckFinished();
        }

        void CheckFinished()
        {
            if (m_FinishedCount >= m_TaskCount && m_Joined)
            {
                DoComplete();
            }
        }

        protected virtual void DoComplete()
        {
            if (completeHandle != null)
            {
                completeHandle();
            }
        }

        public void Clean()
        {
            m_TaskCount = 0;
            m_FinishedCount = 0;
            m_Joined = false;
            completeHandle = null;
        }
    }
}
