using UnityEngine;
using System.Collections.Generic;
using System;

namespace YH.MyInput
{
    class InputManager
    {
        public enum ActiveInputMethod
        {
            Hardware,
            Touch
        }


        private static VirtualInput activeInput;

        private static VirtualInput s_TouchInput;
        private static VirtualInput s_HardwareInput;


        static InputManager()
        {
            s_TouchInput = new Platform.MobileInput();
            s_HardwareInput = new Platform.StandaloneInput();
#if MOBILE_INPUT
            activeInput = s_TouchInput;
#else
            activeInput = s_HardwareInput;
#endif
        }

        public static void SwitchActiveInputMethod(ActiveInputMethod activeInputMethod)
        {
            switch (activeInputMethod)
            {
                case ActiveInputMethod.Hardware:
                    activeInput = s_HardwareInput;
                    break;

                case ActiveInputMethod.Touch:
                    activeInput = s_TouchInput;
                    break;
            }
        }

        public static bool AxisExists(string name)
        {
            return activeInput.AxisExists(name);
        }

        public static bool ButtonExists(string name)
        {
            return activeInput.ButtonExists(name);
        }

        public static void RegisterVirtualAxis(VirtualAxis axis)
        {
            activeInput.RegisterVirtualAxis(axis);
        }


        public static void RegisterVirtualButton(VirtualButton button)
        {
            activeInput.RegisterVirtualButton(button);
        }


        public static void UnRegisterVirtualAxis(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            activeInput.UnRegisterVirtualAxis(name);
        }


        public static void UnRegisterVirtualButton(string name)
        {
            activeInput.UnRegisterVirtualButton(name);
        }


        // returns a reference to a named virtual axis if it exists otherwise null
        public static VirtualAxis VirtualAxisReference(string name)
        {
            return activeInput.VirtualAxisReference(name);
        }


        // returns the platform appropriate axis for the given name
        public static float GetAxis(string name)
        {
            return activeInput.GetAxis(name);
        }


        public static float GetAxisRaw(string name)
        {
            return activeInput.GetAxisRaw(name);
        }


        // -- Button handling --
        public static bool GetButton(string name)
        {
            return activeInput.GetButton(name);
        }


        public static bool GetButtonDown(string name)
        {
            return activeInput.GetButtonDown(name);
        }


        public static bool GetButtonUp(string name)
        {
            return activeInput.GetButtonUp(name);
        }


        public static void SetButtonDown(string name)
        {
            activeInput.SetButtonDown(name);
        }


        public static void SetButtonUp(string name)
        {
            activeInput.SetButtonUp(name);
        }


        public static void SetAxisPositive(string name)
        {
            activeInput.SetAxisPositive(name);
        }


        public static void SetAxisNegative(string name)
        {
            activeInput.SetAxisNegative(name);
        }


        public static void SetAxisZero(string name)
        {
            activeInput.SetAxisZero(name);
        }


        public static void SetAxis(string name, float value)
        {
            activeInput.SetAxis(name, value);
        }


        public static Vector3 mousePosition
        {
            get { return activeInput.MousePosition(); }
        }


        public static void SetVirtualMousePositionX(float f)
        {
            activeInput.SetVirtualMousePositionX(f);
        }


        public static void SetVirtualMousePositionY(float f)
        {
            activeInput.SetVirtualMousePositionY(f);
        }


        public static void SetVirtualMousePositionZ(float f)
        {
            activeInput.SetVirtualMousePositionZ(f);
        }
    }
}
