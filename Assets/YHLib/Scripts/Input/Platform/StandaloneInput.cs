using System;
using UnityEngine;

namespace YH.MyInput.Platform
{
    public class StandaloneInput : VirtualInput
    {
        public override float GetAxis(string name)
        {
            return UnityEngine.Input.GetAxis(name);
        }

        public override float GetAxisRaw(string name)
        {
            return UnityEngine.Input.GetAxisRaw(name);
        }

        public override bool GetButton(string name)
        {
            return UnityEngine.Input.GetButton(name);
        }


        public override bool GetButtonDown(string name)
        {
            return UnityEngine.Input.GetButtonDown(name);
        }


        public override bool GetButtonUp(string name)
        {
            return UnityEngine.Input.GetButtonUp(name);
        }


        public override void SetButtonDown(string name)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override void SetButtonUp(string name)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override void SetAxisPositive(string name)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override void SetAxisNegative(string name)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override void SetAxisZero(string name)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override void SetAxis(string name, float value)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override Vector3 MousePosition()
        {
            return UnityEngine.Input.mousePosition;
        }
    }
}