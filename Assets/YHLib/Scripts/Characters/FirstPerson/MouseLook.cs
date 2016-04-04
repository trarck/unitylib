using System;
using UnityEngine;

namespace YH.Cameras.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        [SerializeField]
        float m_XSensitivity = 2f;

        [SerializeField]
        float m_YSensitivity = 2f;

        [SerializeField]
        bool m_ClampVerticalRotation = true;

        [SerializeField]
        float m_MinimumX = -90F;

        [SerializeField]
        float m_MaximumX = 90F;

        [SerializeField]
        bool m_Smooth;

        [SerializeField]
        float m_SmoothTime = 5f;

        [SerializeField]
        bool m_ActiveOnTouch = true;

#if UNITY_ANDROID
        bool m_TouchStart = false;
#endif

        private Quaternion m_TargetRotation;
        private Quaternion m_CameraRotation;

        Transform m_TargetTransform;
        Transform m_CameraTransform;

        public void Init(Transform target, Transform camera)
        {
            m_TargetTransform = target;
            m_CameraTransform = camera;

            m_TargetRotation = target.localRotation;
            m_CameraRotation = camera.localRotation;
        }

        public void LookRotation()
        {
            //on android the first touch get value is big
#if UNITY_ANDROID
            if (Input.GetMouseButtonUp(0))
            {
                m_TouchStart = false;
            }

            if (m_TouchStart && (!m_ActiveOnTouch || Input.GetMouseButton(0)))
            {
                ParseInput(m_TargetTransform, m_CameraTransform);
            }

            if (Input.GetMouseButtonDown(0))
            {
                m_TouchStart = true;
            }
#else
            if (!m_ActiveOnTouch || Input.GetMouseButton(0))
            {
                ParseInput(m_TargetTransform, m_CameraTransform);
            }
#endif
        }

        void ParseInput(Transform target, Transform camera)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (mouseX != 0 || mouseY != 0)
            {
                float yRot = mouseX * m_XSensitivity;
                float xRot = mouseY * m_YSensitivity;

                m_TargetRotation *= Quaternion.Euler(0f, yRot, 0f);
                m_CameraRotation *= Quaternion.Euler(-xRot, 0f, 0f);

                if (m_ClampVerticalRotation)
                    m_CameraRotation = ClampRotationAroundXAxis(m_CameraRotation);

                if (m_Smooth)
                {
                    target.localRotation = Quaternion.Slerp(target.localRotation, m_TargetRotation,
                        m_SmoothTime * Time.deltaTime);
                    camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraRotation,
                        m_SmoothTime * Time.deltaTime);
                }
                else
                {
                    target.localRotation = m_TargetRotation;
                    camera.localRotation = m_CameraRotation;
                }
            }
        }


        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, m_MinimumX, m_MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
