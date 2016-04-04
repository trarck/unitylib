using UnityEngine;
using System.Collections;

namespace YH
{
    namespace Cameras
    {
        public class FirstPersonCamera : AbstractCamera
        {
            [SerializeField]
            private float m_MoveSpeed = 10f;
            [Range(0f, 10f)]
            [SerializeField]
            private float m_TurnSpeed = 1.5f;
            [SerializeField]
            private bool m_LockCursor = false;

            [SerializeField]
            float m_LookHeight = 1.0f;

            void Awake()
            {

            }

            // Update is called once per frame
            protected override void parseInput(float deltaTime)
            {
                //处理转动
                parseTurn(deltaTime);

                //处理移动
                parseMove(deltaTime);

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

            protected void parseTurn(float deltaTime)
            {
                if (Input.GetMouseButton(0))
                {
                    Transform tf = selfTransform;

                    float mouseX = Input.GetAxis("Mouse X");
                    float mouseY = Input.GetAxis("Mouse Y");

                    //Vector3 eulerAngles=transform.rotation.eulerAngles;
                    float angleY = mouseX * m_TurnSpeed;
                    float angleX = mouseY * -m_TurnSpeed;

                    //tf.Rotate(angleX, angleY, 0);

                    Vector3 e = tf.eulerAngles;
                    e.x += angleX;
                    e.y += angleY;
                    tf.eulerAngles = e;
                }
            }

            protected void parseMove(float deltaTime)
            {
                bool haveTranslate = false;

                Vector3 translate = new Vector3(0f, 0f, 0f);

                if (Input.GetKey("w"))
                {
                    translate.z += deltaTime * m_MoveSpeed;
                    haveTranslate = true;
                }

                if (Input.GetKey("s"))
                {
                    translate.z -= deltaTime * m_MoveSpeed;
                    haveTranslate = true;
                }

                if (Input.GetKey("a"))
                {
                    translate.x -= deltaTime * m_MoveSpeed;
                    haveTranslate = true;
                }

                if (Input.GetKey("d"))
                {
                    translate.x += deltaTime * m_MoveSpeed;
                    haveTranslate = true;
                }

                if (haveTranslate)
                {
                    Transform tf = selfTransform;

                    //transform.Translate(translate);

                    Vector3 offset = translate;// tf.TransformDirection(translate);

                    offset.y = 0;

                    tf.position += offset;
                }
            }
        }
    }
}