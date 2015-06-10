using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

namespace Cameras
{
    public class FreeCamera : AbstractCamera
    {
        [SerializeField] private float m_MoveSpeed = 10f;
        [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 1.5f;
        [SerializeField] private bool m_LockCursor = false; 

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
                Transform transform = GetComponent<Transform>();

                float mouseX = CrossPlatformInputManager.GetAxis("Mouse X");
                float mouseY = CrossPlatformInputManager.GetAxis("Mouse Y");

                //Vector3 eulerAngles=transform.rotation.eulerAngles;
                float angleY = mouseX * m_TurnSpeed;
                float angleX = mouseY * -m_TurnSpeed;

                transform.Rotate(angleX, angleY, 0);
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
                Transform transform = GetComponent<Transform>();

                transform.Translate(translate);
            }
        }
    }
}