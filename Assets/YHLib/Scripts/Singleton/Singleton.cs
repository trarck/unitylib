using UnityEngine;

public class Singleton<T> : MonoBehaviour 
    where T : Component
{
    private static T m_Instance ;

    static Singleton()
    {
        Debug.Log("in static construct");
 
        GameObject obj = new GameObject();
        obj.hideFlags = HideFlags.HideAndDontSave;
        m_Instance=  obj.AddComponent<T>();
    }

    public static T Instance
    {
        get
        {
            return m_Instance;
        }
    }
} 
