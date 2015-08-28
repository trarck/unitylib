using UnityEngine;
using System.Collections;

public class ParallaxNode : MonoBehaviour 
{
    [SerializeField]
    Vector2 m_Ratio=Vector2.zero;

    [SerializeField]
    Vector2 m_Offset=Vector2.zero;

    Transform m_Transform=null;

    void Start()
    {
        m_Transform = GetComponent<Transform>();
    }

    public void setPosition(Vector2 pos)
    {
        m_Transform.localPosition = pos;
    }

    //==========================generate auto==============================//
    public Vector2 ratio
    {
        set
        {
            m_Ratio = value;
        }

        get
        {
            return m_Ratio;
        }
    }

    public Vector2 offset
    {
        set
        {
            m_Offset = value;
        }

        get
        {
            return m_Offset;
        }
    }
}
