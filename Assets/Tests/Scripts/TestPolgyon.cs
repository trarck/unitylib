using UnityEngine;
using System.Collections;

public class TestPolgyon : MonoBehaviour {

    [SerializeField]
    Rect2dRenderer m_Rect2d;

    Vector3 m_DragStartPosition;
    bool m_DragStart;

    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0))
        {
            m_DragStart = false;
        }


        if (m_DragStart)
        {
            if(Vector3.Distance(m_DragStartPosition,Input.mousePosition)>4)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                m_Rect2d.endPosition = pos;
            }            
        }

        if (Input.GetMouseButtonDown(0))
        {
            m_DragStartPosition = Input.mousePosition;
            m_DragStart = true;
            Vector3 pos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            m_Rect2d.startPosition = pos;
        }        
	}
}
