namespace YH.MyInput
{
    public class VirtualAxis
    {
        public string name { get; private set; }
        private float m_Value;
        public bool matchWithInputManager { get; private set; }

        VirtualInput m_VirtualInput;

        public VirtualAxis(string name)
            : this(name, true)
        {
        }


        public VirtualAxis(string name, bool matchToInputSettings)
        {
            this.name = name;
            matchWithInputManager = matchToInputSettings;
        }


        // removes an axes from the cross platform input system
        public void Remove()
        {
            if (m_VirtualInput != null)
            {
                m_VirtualInput.UnRegisterVirtualAxis(name);
            }
        }


        // a controller gameobject (eg. a virtual thumbstick) should update this class
        public void Update(float value)
        {
            m_Value = value;
        }


        public float GetValue
        {
            get { return m_Value; }
        }


        public float GetValueRaw
        {
            get { return m_Value; }
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
