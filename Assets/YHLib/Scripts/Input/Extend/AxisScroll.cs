using System;
using UnityEngine;

namespace YH.MyInput
{
    public class AxisScroll : MonoBehaviour
    {
        public string axis;

	    void Update() { }

	    public void HandleInput(float value)
        {
            InputManager.SetAxis(axis, (value*2f) - 1f);
        }
    }
}
