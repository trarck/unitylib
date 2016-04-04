using UnityEngine;
using System.Collections;
using YH.MyInput;

public class TestInput : MonoBehaviour
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

    void Update()
    {
        ParseInput(Time.deltaTime);
    }

    // Update is called once per frame
    protected void ParseInput(float deltaTime)
    {
        //处理转动
        ParseTurn(deltaTime);

        //处理移动
        ParseMove(deltaTime);

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

    protected void ParseTurn(float deltaTime)
    {
        if (Input.GetMouseButton(0))
        {
            Transform tf = this.transform;
            float mouseX = InputManager.GetAxis("Mouse X");
            float mouseY = InputManager.GetAxis("Mouse Y");

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

    protected void ParseMove(float deltaTime)
    {
#if MOBILE_INPUT
        ParseTouchMove(deltaTime);
#else
        ParseKeyMove(deltaTime);
#endif
    }

    protected void ParseKeyMove(float deltaTime)
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

        if (horizontal !=0 || vertical!=0)
        {
            //Transform tf = this.transform;
            Vector3 desiredMove =this.transform.forward * vertical + this.transform.right * horizontal;
            desiredMove *= m_MoveSpeed * deltaTime;

            this.transform.position += desiredMove;
            //transform.Translate(translate);

            //Vector3 offset = translate;// tf.TransformDirection(translate);

            //offset.y = 0;

            //tf.position += offset;
        }
    }

    protected void ParseTouchMove(float deltaTime)
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

            this.transform.position += desiredMove;
        }
    }
}
