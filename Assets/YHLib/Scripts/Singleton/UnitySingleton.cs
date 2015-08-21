using UnityEngine;
using System.Collections;

public class UnitySingleton<T> : MonoBehaviour
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
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    m_Instance = obj.AddComponent<T>();
                }
            }
            return m_Instance;
        }
    }
}