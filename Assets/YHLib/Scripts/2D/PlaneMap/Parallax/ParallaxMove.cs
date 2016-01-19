using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace YH
{
    public class ParallaxMove : Parallax
    {
        [SerializeField]
        Vector2 m_Velocity = new Vector2(1.0f, 1.0f);

        // Update is called once per frame
        void LateUpdate()
        {

            Vector2 dis = m_Velocity * Time.deltaTime;

            Vector3 pos = m_Transform.localPosition;

            pos.x += dis.x;

            pos.y += dis.y;

            UpdateChidren(pos);

            m_Transform.localPosition = pos;
        }

        public Vector2 velocity
        {
            set
            {
                m_Velocity = value;
            }

            get
            {
                return m_Velocity;
            }
        }
    }
}