using UnityEngine;
using System.Collections;

public class UnitySingletonKeep<T> : MonoBehaviour
    where T : Component
{
    private static T m_Instance;

    public static T Instance
    {
        get
        {
            Debug.Log(m_Instance);
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(T)) as T;
                if (m_Instance == null)
                {
                    Debug.Log("create instance");
                    GameObject obj = new GameObject();
                    //obj.hideFlags = HideFlags.HideAndDontSave;
                    m_Instance = obj.AddComponent<T>();
                }
            }
            return m_Instance;
        }
    }

    public virtual void Awake()
    {
        Debug.Log("On Awake:"+m_Instance);
        DontDestroyOnLoad(this.gameObject);

        if (m_Instance == null)
        {
            Debug.Log("Test null");
            m_Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}