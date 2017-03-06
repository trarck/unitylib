using UnityEngine;
using System.Collections;
using YH.MyInput;

namespace YH.Cameras.FirstPerson
{
    public class SimpleController : MonoBehaviour
    {

        [SerializeField]
        private float m_MoveSpeed = 5f;

        //[SerializeField]
        //private float m_TurnSpeed = 1.5f;

        [SerializeField]
        float m_MovingTurnSpeed = 100;
        [SerializeField]
        float m_StationaryTurnSpeed = 50;

        [SerializeField]
        private bool m_LockCursor = false;

        [SerializeField]
        Camera m_Camera;

        [SerializeField]
        private MouseLook m_MouseLook;

        [SerializeField]
        bool m_CameraFollow;

        CharacterController m_CharacterController;
        Transform m_Transform;

        void Start()
        {
            if (m_Camera == null)
            {
                m_Camera = Camera.main;
            }

            m_CharacterController = GetComponent<CharacterController>();
            m_Transform = GetComponent<Transform>();

            m_MouseLook.Init(m_Transform, m_Camera.transform);
        }

        void Update()
        {
            RotateView();
            CheckCursor();
        }

        void FixedUpdate()
        {
            ParseMove(Time.fixedDeltaTime);
        }

        void CheckCursor()
        {
            if (m_LockCursor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Cursor.visible = false;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Cursor.visible = true;
                }
            }
        }

        protected void ParseMove(float deltaTime)
        {
#if MOBILE_INPUT
            if (m_CameraFollow)
            {
                CameraFollowMove(deltaTime);
            }
            else
            {
                TouchMove(deltaTime);
            }          
#else
        KeyMove(deltaTime);
#endif
        }

        //protected void ParseTurn(float deltaTime)
        //{
        //    if (Input.GetMouseButton(0))
        //    {
        //        float mouseX = InputManager.GetAxis("Mouse X");
        //        float mouseY = InputManager.GetAxis("Mouse Y");

        //        //Vector3 eulerAngles=transform.rotation.eulerAngles;
        //        float angleY = mouseX * m_TurnSpeed;
        //        float angleX = mouseY * -m_TurnSpeed;

        //        //tf.Rotate(angleX, angleY, 0);

        //        Vector3 e = m_Transform.eulerAngles;
        //        e.x += angleX;
        //        e.y += angleY;
        //        m_Transform.eulerAngles = e;
        //    }
        //}            

        protected void KeyMove(float deltaTime)
        {
            float horizontal = 0;
            float vertical = 0;
            if (Input.GetKey("w"))
            {
                vertical = 0.5f;
            }

            if (Input.GetKey("s"))
            {
                vertical = -0.5f;
            }

            if (Input.GetKey("a"))
            {
                horizontal = -0.5f;
            }

            if (Input.GetKey("d"))
            {
                horizontal = 0.5f;
            }

            if (horizontal != 0 || vertical != 0)
            {
                //Transform tf = this.transform;
                Vector3 desiredMove = this.transform.forward * vertical + this.transform.right * horizontal;
                desiredMove *= m_MoveSpeed * deltaTime;

                this.transform.position += desiredMove;
                //transform.Translate(translate);

                //Vector3 offset = translate;// tf.TransformDirection(translate);

                //offset.y = 0;

                //tf.position += offset;
            }
        }

        protected void TouchMove(float deltaTime)
        {
            float horizontal = InputManager.GetAxis("Horizontal");
            float vertical = InputManager.GetAxis("Vertical");

            if (horizontal != 0 || vertical != 0)
            {
                Vector2 input = new Vector2(horizontal, vertical);
                if (input.sqrMagnitude > 1)
                {
                    input.Normalize();
                }

                Vector3 desiredMove = this.transform.forward * input.y + this.transform.right * input.x;

                desiredMove *= m_MoveSpeed * deltaTime;

                m_CharacterController.Move(desiredMove);
            }
        }
        
        void CameraFollowMove(float deltaTime)
        {
            float horizontal = InputManager.GetAxis("Horizontal");
            float vertical = InputManager.GetAxis("Vertical");

            if (horizontal != 0 || vertical != 0)
            {

                float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, vertical);

                float turnAmount = Mathf.Atan2(horizontal, Mathf.Abs(vertical));
                //Debug.Log(turnAmount + "," + horizontal + "," + vertical);
                this.transform.Rotate(0, turnAmount * deltaTime * turnSpeed, 0);

                Vector3 desiredMove = this.transform.forward * vertical;//vertical>0? this.transform.forward:-this.transform.forward;//* vertical;

                desiredMove *= m_MoveSpeed * deltaTime;

                m_CharacterController.Move(desiredMove);
                //this.transform.position += desiredMove;

                //Vector3 desiredMove = this.transform.forward* vertical+this.transform.right*horizontal;

                //desiredMove *= m_MoveSpeed * deltaTime;

                //this.transform.position += desiredMove;

                //float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, vertical);

                //float turnAmount = Mathf.Atan2(horizontal, Mathf.Abs(vertical));
                //Debug.Log(turnAmount + "," + horizontal + "," + vertical);
                //this.transform.Rotate(0, turnAmount * deltaTime * turnSpeed, 0);
            }
        }

        void RotateView()
        {
            m_MouseLook.LookRotation();
        }
    }
}