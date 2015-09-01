using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parallax : MonoBehaviour
{
    List<ParallaxNode> m_Nodes;

    Transform m_Transform;

	// Use this for initialization
	void Start () 
    {
        m_Nodes = new List<ParallaxNode>();
        m_Transform = GetComponent<Transform>();

        InitChidren();
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 pos = m_Transform.position;

        ParallaxNode node;
        float x = 0, y = 0;
        for (int i = 0; i < m_Nodes.Count; ++i)
        {
            node = m_Nodes[i];
            x = -pos.x + pos.x * node.ratio.x + node.offset.x;
            y = -pos.y + pos.y * node.ratio.y + node.offset.y;
            node.setPosition(x,y);
        }
	}

    void InitChidren()
    {
        for (int i = 0, l = m_Transform.childCount; i<l; ++i)
        {
            Transform child = m_Transform.GetChild(i);

            AddNode(child.gameObject);
        }
    }

    public void AddNode(GameObject obj)
    {
        if (obj.activeInHierarchy)
        {
            ParallaxNode node = obj.GetComponent<ParallaxNode>();
            if (node != null)
            {
                if (node.transform.parent != m_Transform)
                {
                    node.transform.SetParent(m_Transform);
                }

                SetNodePosition(node);

                m_Nodes.Add(node);
            }
        }
    }

    public void AddNode(GameObject obj,Vector2 ratio,Vector2 offset)
    {
        ParallaxNode node = obj.GetComponent<ParallaxNode>();
        if(node==null){
            node = obj.AddComponent<ParallaxNode>();
        }
        
        node.ratio=ratio;
        node.offset=offset;

        AddNode(obj);
    }

    void SetNodePosition(ParallaxNode node)
    {
        Vector3 pos = m_Transform.position;

        float x = -pos.x + pos.x * node.ratio.x + node.offset.x;
        float y = -pos.y + pos.y * node.ratio.y + node.offset.y;

        node.setPosition(x, y);
    }

    public void RemoveNode(ParallaxNode node)
    {
        m_Nodes.Remove(node);
        node.transform.SetParent(null);
    }
}
