using UnityEngine;
using System.Collections;

/// <summary>
/// 只在一个场景中
/// </summary>
/// <typeparam name="T"></typeparam>
public class UnitySingletonOnce<T> : MonoBehaviour
    where T : Component
{
    private static T m_Instance;

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(T)) as T;
                if (m_Instance == null)
                { 
                    GameObject singletonObj = new GameObject();
                    singletonObj.name = "(singleton) " + typeof(T).ToString();
                    m_Instance = singletonObj.AddComponent<T>();
                }
            }
            return m_Instance;
        }
    }

    public static void DestroyInstance()
    {
        Destroy(m_Instance);
        m_Instance = null;
    }
}