﻿using UnityEngine;
using System.Collections;

namespace YH
{
    public class UnitySingleton<T> : MonoBehaviour
        where T : Component
    {
        private static T m_Instance;

        static bool m_IsDestroy = false;

        public static T Instance
        {
            get
            {
                if (m_Instance == null && !m_IsDestroy)
                {
                    Debug.Log("####Find instance:" + typeof(T).ToString());
                    m_Instance = FindObjectOfType(typeof(T)) as T;
                    if (m_Instance == null)
                    {
                        Debug.Log("####create instance:" + typeof(T).ToString());

                        GameObject singletonObj = new GameObject();
                        singletonObj.name = "(singleton) " + typeof(T).ToString();
                        if (Application.isPlaying)
                        {
                            //方法一
                            DontDestroyOnLoad(singletonObj);
                        }
                        //方法二
                        //DontSave标志表示不会在加载新场景删除，所以不用DontDestroyOnLoad
                        //singletonObj.hideFlags = HideFlags.HideAndDontSave;

                        //Debug.Log("add instance before");
                        m_Instance = singletonObj.AddComponent<T>();

                        //Debug.Log("add instance after");
                    }
                }
                return m_Instance;
            }
        }

        protected virtual void Awake()
        {
            Debug.Log("####Awake instance:" + typeof(T).ToString());
            m_Instance = this as T;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected void OnDestroy()
        {
            m_IsDestroy = true;
        }

        public static void DestroyInstance()
        {
            Destroy(m_Instance);
            m_Instance = null;
        }
    }
}