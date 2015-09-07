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

    float m_LeftCheckSize = 0;
    float m_RightCheckSize = 0;

    Vector3 m_StartPosition=Vector3.zero;

    Vector3 m_LastPosition = Vector3.zero;

    List<Transform> m_DisplayQueue;

    List<Transform> m_Elements;

    Transform m_Transform = null;

    void Awake()
    {
        m_Transform = GetComponent<Transform>();
    }

	void Start () 
    {
        
        m_Elements = new List<Transform>();
        m_DisplayQueue = new List<Transform>();

        m_LeftCheckSize = m_ElementSize * 0.4f;
        m_RightCheckSize = m_ElementSize * 0.4f;

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
        Vector3 pos = m_Transform.InverseTransformPoint(m_Target.position);
        if (m_LastPosition != pos)
        {
            Move(pos.x - m_LastPosition.x);
            m_LastPosition = pos;
        }
    }

    /// <summary>
    /// 自动扩展用于子项内容一样的情况。
    /// 直接使用子元素更灵活，每个子元素只要大小一样，里面的内容可以不一样。
    /// </summary>
    void CreateElementList()
    {
        int l = m_Transform.childCount;

        if (l == 1)
        {
            //复制

            //检查要复制多少个.至少比屏幕数大1.这晨的count正好是复制的数
            int count = (int)Mathf.Ceil(m_TargetHalfSize * 2.0f / m_ElementSize);
            GameObject obj = m_Transform.GetChild(0).gameObject;

            AddElement(obj.transform);

            for (int i = 0; i < count; ++i)
            {
                GameObject newObj = Instantiate(obj);
                newObj.transform.SetParent(m_Transform);

                AddElement(newObj.transform);
            }
        }
        else
        {
            //直接使用
            for (int i = 0; i < l; ++i)
            {
                Transform child = m_Transform.GetChild(i);

                AddElement(child);
            }
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

            //Debug.Log("r:"+m_TargetRightInLocal.x + "," + m_RightPosition + "," + (m_RightPosition - m_TargetRightInLocal.x));

            if (m_RightPosition - m_TargetRightInLocal.x <= m_RightCheckSize)
            {
                Transform ele = m_DisplayQueue[0];

                SetElementPosition(ele, m_RightPosition);

                m_DisplayQueue.Remove(ele);

                m_DisplayQueue.Add(ele);

                m_RightPosition += m_ElementSize;
                m_LeftPosition += m_ElementSize;
            }
        }
        else
        {
            //向左移动。比较左边
            Vector3 targetLeftEdge = m_Target.position + new Vector3(-m_TargetHalfSize, 0, 0);
            Vector3 m_TargetLeftInLocal = m_Transform.InverseTransformPoint(targetLeftEdge);

            //Debug.Log("l:"+m_TargetLeftInLocal.x + "," + m_LeftPosition + "," + (m_TargetLeftInLocal.x - m_LeftPosition));
            if (m_TargetLeftInLocal.x - m_LeftPosition <= m_LeftCheckSize)
            {
                m_LeftPosition -= m_ElementSize;
                m_RightPosition -= m_ElementSize;

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

    void OnDrawGizmos()
    {
        Vector3 size;
        size.x = m_ElementSize;
        size.y = 2.0f * Camera.main.orthographicSize;
        size.z = 1.0f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
