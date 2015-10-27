using UnityEngine;

namespace YH
{
    public class Singleton<T>  where T : class,new()
    {
        private static T m_Instance;

        static Singleton()
        {
            Debug.Log("in static construct");

            if (m_Instance == null)
            {
                m_Instance = new T();
            }
        }

        //public Singleton()
        //{
        //    Debug.Log("in construct");
        //}

        public static T Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public static void DestroyInstance()
        {
            m_Instance = null;
        }
    }
}