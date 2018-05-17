using UnityEngine;

namespace YH
{
 
    public class SingletonLock<T> where T : class,new()
    {
        private static T m_Instance;
        private static readonly object syslock = new object();

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (syslock)
                    {
                        if (m_Instance == null)
                        {
                            m_Instance = new T();
                        }
                    }
                }
                return m_Instance;
            }
        }

        public void DestroyInstance()
        {
            m_Instance = null;
        }
    }
}