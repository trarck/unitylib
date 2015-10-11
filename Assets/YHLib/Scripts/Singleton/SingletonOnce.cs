using UnityEngine;

namespace YH
{ }
public class SingletonOnce<T> : MonoBehaviour 
    where T : Component
{
    private static T m_Instance ;

    static SingletonOnce()
    {
        Debug.Log("in static construct");

        GameObject singletonObj = new GameObject();

        singletonObj.name = "(singleton) " + typeof(T).ToString();

        m_Instance = singletonObj.AddComponent<T>();
    }

    public static T Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public static void DestroyInstance()
    {
        Destroy(m_Instance);
        m_Instance = null;
    }
} 
