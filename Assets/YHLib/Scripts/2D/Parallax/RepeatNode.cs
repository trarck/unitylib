using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RepeatNode : ParallaxNode {

    [SerializeField]
    float m_ElementSize = 8.0f;

    Queue<Transform> m_Elements;

	void Start () 
    {
        base.Start();
        m_Elements = new Queue<Transform>();

        InitElements();
	}

    void Update()
    {

    }

    void InitElements()
    {
        for (int i = 0, l = m_Transform.childCount; i < l; ++i)
        {
            Transform child = m_Transform.GetChild(i);

            AddElement(child);
        }
    }
	
	public void AddElement (Transform element) 
    {
        m_Elements.Enqueue(element);
	}
}
