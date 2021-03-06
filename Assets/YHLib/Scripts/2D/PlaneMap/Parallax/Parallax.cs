﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace YH
{
    public class Parallax : MonoBehaviour
    {
        protected List<ParallaxNode> m_Nodes;

        protected Transform m_Transform;
        protected Vector3 m_LastPosition = Vector3.zero;
        // Use this for initialization
        void Awake()
        {
            m_Transform = GetComponent<Transform>();
        }

        void Start()
        {
            m_Nodes = new List<ParallaxNode>();

            InitChidren();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Vector3 pos = m_Transform.localPosition;
            if (m_LastPosition != pos)
            {
                UpdateChidren(pos);
                m_LastPosition = pos;
            }
        }

        protected void InitChidren()
        {
            for (int i = 0, l = m_Transform.childCount; i < l; ++i)
            {
                Transform child = m_Transform.GetChild(i);

                AddNode(child.gameObject);
            }
        }

        protected void UpdateChidren(Vector3 pos)
        {
            ParallaxNode node;
            float x = 0, y = 0;
            for (int i = 0; i < m_Nodes.Count; ++i)
            {
                node = m_Nodes[i];
                x = -pos.x + pos.x * node.ratio.x + node.offset.x;
                y = -pos.y + pos.y * node.ratio.y + node.offset.y;
                node.setPosition(x, y);
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

        public void AddNode(GameObject obj, Vector2 ratio, Vector2 offset)
        {
            ParallaxNode node = obj.GetComponent<ParallaxNode>();
            if (node == null)
            {
                node = obj.AddComponent<ParallaxNode>();
            }

            node.ratio = ratio;
            node.offset = offset;

            AddNode(obj);
        }

        protected void SetNodePosition(ParallaxNode node)
        {
            Vector3 pos = m_Transform.localPosition;

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
}