using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH.Timeline
{
    public class Clip : IClip
    {
        private double m_Start = 0;
        private double m_End = 0;

        public double start
        {
            get
            {
                return m_Start;
            }
            set
            {
                m_Start = value;
            }
        }

        public double end {
            get
            {
                return m_End;
            }
            set
            {
                m_End = value;
            }
        }

        public double duration
        {
            get
            {
                return m_End - m_Start;
            }
        }

    }
}