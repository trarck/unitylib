using UnityEngine;
using System.Collections;

public class ParallaxNode : MonoBehaviour 
{
    [SerializeField]
    protected Vector2 m_Ratio=Vector2.zero;

    [SerializeField]
    protected Vector2 m_Offset = Vector2.zero;

    protected Transform m_Transform = null;

    protected void Start()
    {
        m_Transform = GetComponent<Transform>();
    }

    public void setPosition(Vector2 pos)
    {
        m_Transform.localPosition = pos;
    }

    public void setPosition(float x,float y)
    {
        m_Transform.localPosition = new Vector3(x, y, 0);
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
