﻿using UnityEngine;

public class Singleton<T> : MonoBehaviour 
    where T : Component
{
    private static T m_Instance ;

    static Singleton()
    {
        Debug.Log("in static construct");

        GameObject singletonObj = new GameObject();

        //方法一
        //singletonObj.name = "(singleton) " + typeof(T).ToString();
        //DontDestroyOnLoad(singletonObj);

        //方法二
        //DontSave标志表示不会在加载新场景删除，所以不用DontDestroyOnLoad
        singletonObj.hideFlags = HideFlags.HideAndDontSave;

        //Debug.Log("add instance before");
        m_Instance = singletonObj.AddComponent<T>();
    }

    public static T Instance
    {
        get
        {
            return m_Instance;
        }
    }

    //public static void DestroyInstance()
    //{
    //    Destroy(m_Instance);
    //    m_Instance = null;
    //}
} 