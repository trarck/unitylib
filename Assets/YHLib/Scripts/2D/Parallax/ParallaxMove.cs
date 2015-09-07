using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxMove : Parallax
{
    [SerializeField]
    Vector2 m_Speed= new Vector2(1.0f,1.0f);

	// Update is called once per frame
	void LateUpdate () 
    {

        Vector2 dis = m_Speed * Time.deltaTime;

        Vector3 pos = m_Transform.position;

        pos.x += dis.x;
        
        pos.y += dis.y;

        UpdateChidren(pos);

        m_Transform.position = pos;
	}
}
