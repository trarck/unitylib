using System;
using UnityEngine;

namespace YH
{
    namespace Cameras
    {
        public abstract class AbstractCamera : MonoBehaviour
        {

            public enum UpdateType // The available methods of updating are:
            {
                Update,//normal update
                FixedUpdate, // Update in FixedUpdate (for tracking rigidbodies).
                LateUpdate, // Update in LateUpdate. (for tracking objects that are moved in Update)
                ManualUpdate, // user must call to update camera
            }

            [SerializeField]
            private UpdateType m_UpdateType;

            [SerializeField]
            protected Transform m_Transform = null;

            void Update()
            {
                if (m_UpdateType == UpdateType.Update)
                {
                    parseInput(Time.deltaTime);
                }
            }

            void FixedUpdate()
            {
                if (m_UpdateType == UpdateType.FixedUpdate)
                {
                    parseInput(Time.deltaTime);
                }
            }


            void LateUpdate()
            {
                if (m_UpdateType == UpdateType.LateUpdate)
                {
                    parseInput(Time.deltaTime);
                }
            }


            void ManualUpdate()
            {
                if (m_UpdateType == UpdateType.ManualUpdate)
                {
                    parseInput(Time.deltaTime);
                }
            }

            

            protected abstract void parseInput(float deltaTime);

            public Transform selfTransform
            {
                set
                {
                    m_Transform = value;
                }

                get
                {
                    if (m_Transform == null)
                    {
                        m_Transform = this.transform;
                    }
                    return m_Transform;
                }
            }
        }
    }
}