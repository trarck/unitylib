using System;
using UnityEngine;

namespace YH.MyInput
{
    public class ButtonHandler : MonoBehaviour
    {

        public string Name;

        void OnEnable()
        {

        }

        public void SetDownState()
        {
            InputManager.SetButtonDown(Name);
        }


        public void SetUpState()
        {
            InputManager.SetButtonUp(Name);
        }


        public void SetAxisPositiveState()
        {
            InputManager.SetAxisPositive(Name);
        }


        public void SetAxisNeutralState()
        {
            InputManager.SetAxisZero(Name);
        }


        public void SetAxisNegativeState()
        {
            InputManager.SetAxisNegative(Name);
        }

        public void Update()
        {

        }
    }
}
