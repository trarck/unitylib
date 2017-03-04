using UnityEngine;
using System.Collections;

namespace YH.MyInput
{
    public class MouseInput : MonoBehaviour
    {
        // Options for which axes to use
        public enum AxisOption
        {
            Both, // Use both
            OnlyHorizontal, // Only horizontal
            OnlyVertical // Only vertical
        }

        public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
        public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
        public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input
        public string buttonName = "Mouse"; // The name given to the vertical axis for the cross platform input

        bool m_UseX; // Toggle for using the x axis
        bool m_UseY; // Toggle for using the Y axis
        VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
        VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input
        VirtualButton m_MouseButton;
        // Use this for initialization
#if UNITY_ANDROID
        bool m_TouchDown = false;
#endif
        void Start()
        {
            CreateVirtualAxes();
            m_MouseButton = new VirtualButton(buttonName);
            InputManager.RegisterVirtualButton(m_MouseButton);
        }

        // Update is called once per frame
        void Update()
        {

#if UNITY_ANDROID
            if (Input.GetMouseButton(0) && m_TouchDown)
#else
            if (Input.GetMouseButton(0)
#endif
                {
                if (m_UseX)
                {
                    m_HorizontalVirtualAxis.Update(Input.GetAxis("Mouse X"));
                }

                if (m_UseY)
                {
                    m_VerticalVirtualAxis.Update(Input.GetAxis("Mouse Y"));
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
#if UNITY_ANDROID
                m_TouchDown = true;
#endif
                m_MouseButton.Pressed();
            }

            if (Input.GetMouseButtonUp(0))
            {
#if UNITY_ANDROID
                m_TouchDown = false;
#endif
                m_MouseButton.Released();
            }
        }

        void CreateVirtualAxes()
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
    }
}