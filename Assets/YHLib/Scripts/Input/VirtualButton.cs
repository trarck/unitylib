using UnityEngine;

namespace YH.MyInput
{
    public class VirtualButton
    {
        public string name { get; private set; }
        public bool matchWithInputManager { get; private set; }

        private int m_LastPressedFrame = -5;
        private int m_ReleasedFrame = -5;
        private bool m_Pressed;

        VirtualInput m_VirtualInput;

        public VirtualButton(string name)
            : this(name, true)
        {

        }


        public VirtualButton(string name, bool matchToInputSettings)
        {
            this.name = name;
            matchWithInputManager = matchToInputSettings;
        }


        // A controller gameobject should call this function when the button is pressed down
        public void Pressed()
        {
            if (m_Pressed)
            {
                return;
            }
            m_Pressed = true;
            m_LastPressedFrame = Time.frameCount;
        }


        // A controller gameobject should call this function when the button is released
        public void Released()
        {
            m_Pressed = false;
            m_ReleasedFrame = Time.frameCount;
        }


        // the controller gameobject should call Remove when the button is destroyed or disabled
        public void Remove()
        {
            if (m_VirtualInput != null)
            {
                m_VirtualInput.UnRegisterVirtualButton(this.name);
            }
        }


        // these are the states of the button which can be read via the cross platform input system
        public bool GetButton
        {
            get { return m_Pressed; }
        }


        public bool GetButtonDown
        {
            get
            {
                return m_LastPressedFrame - Time.frameCount == -1;
            }
        }


        public bool GetButtonUp
        {
            get
            {
                return (m_ReleasedFrame == Time.frameCount - 1);
            }
        }

        public VirtualInput virtualInput
        {
            set
            {
                m_VirtualInput = value;
            }

            get
            {
                return m_VirtualInput;
            }
        }
    }
}
