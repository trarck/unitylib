using System;
using UnityEngine;
using YH.MyInput;
using Random = UnityEngine.Random;

namespace YH.Cameras.FirstPerson
{
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField]
        private bool m_IsWalking;
        [SerializeField]
        private float m_WalkSpeed;
        [SerializeField]
        private float m_RunSpeed;
        [SerializeField]
        private MouseLook m_MouseLook;

        [SerializeField]
        private Camera m_Camera;
        private Vector2 m_Input;
        private Vector3 m_OriginalCameraPosition;

        Transform m_Transform;

        void Awake()
        {
            m_Transform = this.transform;
        }

        // Use this for initialization
        private void Start()
        {
            if (m_Camera == null)
            {
                m_Camera = Camera.main;
            }

            m_OriginalCameraPosition = m_Camera.transform.localPosition;

            m_MouseLook.Init(m_Transform, m_Camera.transform);
        }


        // Update is called once per frame
        private void Update()
        {
            RotateView();
        }

        private void FixedUpdate()
        {
            ParseMove(Time.fixedDeltaTime);
        }

        void ParseMove(float deltaTime)
        {
            float speed;
            GetInput(out speed);

            Vector3 desiredMove = m_Transform.forward * m_Input.y + m_Transform.right * m_Input.x;

            desiredMove *= speed * deltaTime;

            m_Transform.position += desiredMove;
        }

        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = InputManager.GetAxis("Horizontal");
            float vertical = InputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation();
        }
    }
}
