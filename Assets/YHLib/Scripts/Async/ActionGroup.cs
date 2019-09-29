using System;
using System.Collections.Generic;
using System.Text;

namespace YH.Async
{
    public class ActionGroup
    {
        public Action completeHandle;

        int m_ActionCount=0;
        int m_FinishedCount = 0;
        bool m_Joined = false;

        public ActionGroup()
        {
            m_ActionCount = 0;
        }

        public ActionGroup(int total)
        {
            m_ActionCount = total;
        }

        public void DoActionFinish()
        {
            ++m_FinishedCount;
            //check finished
            CheckFinished();
        }

        public void Increase()
        {
            ++m_ActionCount;
        }

        public void Increase(int count)
        {
            m_ActionCount+=count;
        }

        public void Decrease()
        {
            --m_ActionCount;
        }

        public void Decrease(int count)
        {
            m_ActionCount-=count;
        }

        public void Join()
        {
            m_Joined = true;
            //check finished
            CheckFinished();
        }

        void CheckFinished()
        {
            if (m_FinishedCount >= m_ActionCount && m_Joined)
            {
                if (completeHandle != null)
                {
                    completeHandle();
                }
            }
        }

        public void Clean()
        {
            m_ActionCount = 0;
            m_FinishedCount = 0;
            m_Joined = false;
            completeHandle = null;
        }
    }
}
