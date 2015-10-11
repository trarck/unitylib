using UnityEngine;
using System.Collections;
using System;

namespace YH
{

    public enum AnimationEventType
    {
        AnimationStart = 1,
        AnimationComplete,


    }

    public class AnimationEventArgs : EventArgs
    {

        private int m_Message;

        public AnimationEventArgs(int message)
        {
            m_Message = message;
        }

        public int message
        {
            get { return m_Message; }
        }
    }

    public class AnimationEventSystem : MonoBehaviour
    {

        public delegate void AnimationEventHandler(AnimationEventArgs e);

        public event AnimationEventHandler OnAnimationEvent;

        void ReceiveEvent(int type)
        {
            if (OnAnimationEvent != null)
            {
                OnAnimationEvent(new AnimationEventArgs(type));
            }

        }

        public void TriggerCompleteEvent()
        {
            if (OnAnimationEvent != null)
            {
                OnAnimationEvent(new AnimationEventArgs((int)AnimationEventType.AnimationComplete));
            }
        }
    }
}