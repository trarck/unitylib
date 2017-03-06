using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YH.MyInput
{
    public class CircleJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public enum AxisOption
        {
            // Options for which axes to use
            Both, // Use both
            OnlyHorizontal, // Only horizontal
            OnlyVertical // Only vertical
        }

        //appear
        public float appearRange = 200;
        public Vector2 appearOrigin = new Vector2(200, 200);

        //move range
        public float movementRange = 100;
        public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
        public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
        public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

        [SerializeField]
        protected Transform m_Touch;

        [SerializeField]
        protected Transform m_Area;

        [SerializeField]
        protected float m_AreaRadius = 92;

        protected Vector3 m_StartPos;
        protected bool m_UseX; // Toggle for using the x axis
        protected bool m_UseY; // Toggle for using the Y axis
        protected VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
        protected VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

        protected float m_AppearRangeSqr;
        protected float m_MovementRangeSqr;
        protected Vector3 m_TouchStartPosition;
        protected Vector3 m_TouchInitPosition;

        protected float m_UIScale;

        void OnEnable()
        {
            CreateVirtualAxes();
        }

        void Start()
        {
            m_UIScale = CalcUIScale();

            m_StartPos = m_Area.position;
            m_TouchInitPosition = m_Touch.position;
            appearRange *= m_UIScale;
            movementRange *= m_UIScale;
            m_AreaRadius *= m_UIScale;
            appearOrigin *= m_UIScale;

            m_AppearRangeSqr = appearRange * appearRange;
            m_MovementRangeSqr = movementRange * movementRange;
            
        }

        void UpdateVirtualAxes(Vector2 delta)
        {
            delta /= movementRange;

            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Update(delta.x);
            }

            if (m_UseY)
            {
                m_VerticalVirtualAxis.Update(delta.y);
            }
        }

        void UpdateVirtualAxes(Vector3 value)
        {
            Vector2 delta = value - m_TouchStartPosition;
            UpdateVirtualAxes(delta);
        }

        protected void CreateVirtualAxes()
        {
            // set axes to use
            m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
            m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

            // create new axes based on axes to use
            if (m_UseX)
            {
                m_HorizontalVirtualAxis = new VirtualAxis(horizontalAxisName);
                InputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
            }
            if (m_UseY)
            {
                m_VerticalVirtualAxis = new VirtualAxis(verticalAxisName);
                InputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
            }
        }


        public void OnDrag(PointerEventData data)
        {
            Vector3 newPos = Vector3.zero;

            Vector2 delta = data.position - new Vector2(m_TouchStartPosition.x, m_TouchStartPosition.y);
            float disSqr = delta.sqrMagnitude;

            if (disSqr > m_MovementRangeSqr)
            {
                delta = delta.normalized * movementRange;
            }

            if (m_UseX)
            {
                newPos.x = delta.x;
            }

            if (m_UseY)
            {
                newPos.y = delta.y;
            }

            m_Touch.position = new Vector3(m_TouchStartPosition.x + newPos.x, m_TouchStartPosition.y + newPos.y, m_TouchStartPosition.z + newPos.z);
            UpdateVirtualAxes(delta);
        }

        public void OnPointerDown(PointerEventData data)
        {
            m_UIScale = CalcUIScale();

            //检查并设置出现位置
            Vector3 pos = data.position;

            Vector2 appearPosition = data.position ;

            if (appearPosition.x - m_AreaRadius  < 0)
            {
                appearPosition.x = m_AreaRadius;
            }
            else if (appearPosition.x + m_AreaRadius  > Camera.main.pixelWidth)
            {
                appearPosition.x = Camera.main.pixelWidth - m_AreaRadius ;
            }

            if (appearPosition.y - m_AreaRadius  < 0)
            {
                appearPosition.y = m_AreaRadius ;
            }
            else if (appearPosition.y + m_AreaRadius > Camera.main.pixelHeight)
            {
                appearPosition.y = Camera.main.pixelHeight - m_AreaRadius ;
            }

            //Debug.Log(data.position + "," + appearPosition);

            if ((appearPosition - appearOrigin).sqrMagnitude < m_AppearRangeSqr)
            {
                m_Area.position = appearPosition;
                m_TouchStartPosition = m_Touch.position;
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            m_Area.position = m_StartPos;
            m_Touch.position = m_TouchInitPosition;
            UpdateVirtualAxes(m_TouchStartPosition);
        }

        void OnDisable()
        {
            // remove the joysticks from the cross platform input
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Remove();
            }
            if (m_UseY)
            {
                m_VerticalVirtualAxis.Remove();
            }
        }

        float CalcUIScale()
        {
            CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
            if (canvasScaler.matchWidthOrHeight == 0)
            {
                return Camera.main.pixelWidth / canvasScaler.referenceResolution.x;
            }
            else if (canvasScaler.matchWidthOrHeight == 1)
            {
                return Camera.main.pixelHeight / canvasScaler.referenceResolution.y;
            }

            return 1;
        }
    }
}