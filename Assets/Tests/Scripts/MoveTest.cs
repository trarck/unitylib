using UnityEngine;
using System.Collections;

public class MoveTest : MonoBehaviour {

    [SerializeField]
    float m_Speed=1.0f;

    [SerializeField]
    bool m_AutoMove = false;

    Transform m_Transform;
	// Use this for initialization
	void Start () {
        m_Transform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () 
    {

        if (m_AutoMove || Input.GetButton("Fire1"))
        {

            float mouseX = m_AutoMove?1.0f:Input.GetAxis("Mouse X");

            Vector3 pos = m_Transform.position;

            pos.x += mouseX*m_Speed * Time.deltaTime;

            m_Transform.position = pos;
        }
	}
}
