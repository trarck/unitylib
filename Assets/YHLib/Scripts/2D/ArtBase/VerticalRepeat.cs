using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VerticalRepeat : MonoBehaviour
{

    [SerializeField]
    float m_ElementSize = 8.0f;

    //初始位置
    [SerializeField]
    float m_ElementStart = 0;

    [SerializeField]
    Transform m_Target;

    [SerializeField]
    float m_TargetHalfSize;

    //左边的位置
    float m_LeftPosition = 0;
    
    //右边的位置
    float m_RightPosition = 0;

    float m_CheckSize = 0;
    float m_LeftCheckSize = 0;
    float m_RightCheckSize = 0;

    Vector3 m_StartPosition=Vector3.zero;

    Vector3 m_LastPosition = Vector3.zero;

    List<Transform> m_DisplayQueue;

    List<Transform> m_Elements;

    Transform m_Transform = null;

	void Start () 
    {
        m_Transform = GetComponent<Transform>();

        m_Elements = new List<Transform>();
        m_DisplayQueue = new List<Transform>();

        m_CheckSize = m_ElementSize * 0.4f;
        m_LeftCheckSize = m_ElementSize * 0.4f;
        m_RightCheckSize = m_ElementSize * 0.6f;

        if (m_Target == null)
        {
            m_Target = Camera.main.transform;
            m_TargetHalfSize = Camera.main.aspect * Camera.main.orthographicSize;
        }
        else if (m_TargetHalfSize == 0)
        {
            Camera camera = m_Target.GetComponent<Camera>();
            if (camera)
            {
                m_TargetHalfSize = camera.aspect * camera.orthographicSize;
            }
        }       

        CreateElementList();

        InitDisplayQueue();
	}

    void Update()
    {
        Vector3 pos = m_Target.position;
        if (m_LastPosition != pos)
        {
            Move(pos.x - m_LastPosition.x);
            m_LastPosition = pos;
        }
    }

    void CreateElementList()
    {
        for (int i = 0, l = m_Transform.childCount; i < l; ++i)
        {
            Transform child = m_Transform.GetChild(i);

            AddElement(child);
        }
    }
	
	public void AddElement (Transform element) 
    {
        m_Elements.Add(element);
	}

    public void InitDisplayQueue()
    {
        m_LeftPosition = m_ElementStart;
        m_RightPosition = m_ElementStart;

        for (int i = 0, l = m_Elements.Count; i < l; ++i)
        {
            Transform ele = m_Elements[i];
            m_DisplayQueue.Add(ele);

            SetElementPosition(ele, m_RightPosition);

            m_RightPosition += m_ElementSize;
        }

        m_StartPosition = m_Target.position;
        m_LastPosition = m_Target.position;
    }

    public void Reset()
    {
        m_DisplayQueue.Clear();
        InitDisplayQueue();
    }

    public void ReLook()
    {
        m_StartPosition = m_Target.position;
        m_LastPosition = m_Target.position;
    }

    void Move(float delta)
    {
        if (delta > 0)
        {
            //向右移动。比较右边
            Vector3 targetRightEdge = m_Target.transform.position + new Vector3(m_TargetHalfSize, 0, 0);
            Vector3 m_TargetRightInLocal = m_Transform.InverseTransformPoint(targetRightEdge);
            if (m_RightPosition - m_TargetRightInLocal.x <= m_CheckSize)
            {
                Transform ele = m_DisplayQueue[0];

                SetElementPosition(ele, m_RightPosition);

                m_DisplayQueue.Remove(ele);

                m_DisplayQueue.Add(ele);

                m_RightPosition += m_ElementSize;
            }
        }
        else
        {
            //向左移动。比较左边
            Vector3 targetLeftEdge = m_Target.position + new Vector3(-m_TargetHalfSize, 0, 0);
            Vector3 m_TargetLeftInLocal = m_Transform.InverseTransformPoint(targetLeftEdge);

            if (m_TargetLeftInLocal.x - m_LeftPosition <= m_CheckSize)
            {
                m_LeftPosition -= m_ElementSize;

                Transform ele = m_DisplayQueue[m_DisplayQueue.Count - 1];

                SetElementPosition(ele, m_LeftPosition);

                m_DisplayQueue.Remove(ele);

                m_DisplayQueue.Insert(0, ele);
            }
        }
    }

    void SetElementPosition(Transform ele,float x)
    {
        Vector3 pos = ele.localPosition;
        pos.x = x;
        ele.localPosition = pos;
    }
}
