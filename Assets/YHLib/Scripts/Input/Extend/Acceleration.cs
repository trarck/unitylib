using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YH.MyInput
{
    // helps with managing tilt input on mobile devices
    public class Acceleration : MonoBehaviour
    {
        // options for the various orientations
        public enum AxisOptions
        {
            ForwardAxis,
            SidewaysAxis,
        }


        [Serializable]
        public class AxisMapping
        {
            public enum MappingType
            {
                NamedAxis,
                MousePositionX,
                MousePositionY,
                MousePositionZ
            };


            public MappingType type;
            public string axisName;
        }


        public AxisMapping mapping;
        public AxisOptions tiltAroundAxis = AxisOptions.ForwardAxis;
        public float fullTiltAngle = 25;
        public float centreAngleOffset = 0;


        private VirtualAxis m_SteerAxis;


        private void OnEnable()
        {
            if (mapping.type == AxisMapping.MappingType.NamedAxis)
            {
                m_SteerAxis = new VirtualAxis(mapping.axisName);
                InputManager.RegisterVirtualAxis(m_SteerAxis);
            }
        }


        private void Update()
        {
            float angle = 0;
            if (UnityEngine.Input.acceleration != Vector3.zero)
            {
                switch (tiltAroundAxis)
                {
                    case AxisOptions.ForwardAxis:
                        angle = Mathf.Atan2(UnityEngine.Input.acceleration.x, -UnityEngine.Input.acceleration.y)*Mathf.Rad2Deg +
                                centreAngleOffset;
                        break;
                    case AxisOptions.SidewaysAxis:
                        angle = Mathf.Atan2(UnityEngine.Input.acceleration.z, -UnityEngine.Input.acceleration.y)*Mathf.Rad2Deg +
                                centreAngleOffset;
                        break;
                }
            }

            float axisValue = Mathf.InverseLerp(-fullTiltAngle, fullTiltAngle, angle)*2 - 1;
            switch (mapping.type)
            {
                case AxisMapping.MappingType.NamedAxis:
                    m_SteerAxis.Update(axisValue);
                    break;
                case AxisMapping.MappingType.MousePositionX:
                    InputManager.SetVirtualMousePositionX(axisValue*Screen.width);
                    break;
                case AxisMapping.MappingType.MousePositionY:
                    InputManager.SetVirtualMousePositionY(axisValue*Screen.width);
                    break;
                case AxisMapping.MappingType.MousePositionZ:
                    InputManager.SetVirtualMousePositionZ(axisValue*Screen.width);
                    break;
            }
        }


        private void OnDisable()
        {
            m_SteerAxis.Remove();
        }
    }
}


namespace YH.MyInput.Inspector
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof (Acceleration.AxisMapping))]
    public class AccelerationAxisStylePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float x = position.x;
            float y = position.y;
            float inspectorWidth = position.width;

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var props = new[] {"type", "axisName"};
            var widths = new[] {.4f, .6f};
            if (property.FindPropertyRelative("type").enumValueIndex > 0)
            {
                // hide name if not a named axis
                props = new[] {"type"};
                widths = new[] {1f};
            }
            const float lineHeight = 18;
            for (int n = 0; n < props.Length; ++n)
            {
                float w = widths[n]*inspectorWidth;

                // Calculate rects
                Rect rect = new Rect(x, y, w, lineHeight);
                x += w;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative(props[n]), GUIContent.none);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
#endif
}
