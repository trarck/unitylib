using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using YHEditor;

namespace YHEditor.Timeline
{
    public class TestTimelineControl : EditorWindow
    {
        TimelineControl m_TimelineControl;

        float m_LeftStateWeightA = 0;
        float m_LeftStateWeightB = 1;
        float m_LeftStateTimeA = 0;
        float m_LeftStateTimeB = 1;

        float m_RightStateWeightA = 0;
        float m_RightStateWeightB = 1;
        float m_RightStateTimeA = 0;
        float m_RightStateTimeB = 1;

        List<TimelineControl.PivotSample> m_SrcPivotList = new List<TimelineControl.PivotSample>();
        List<TimelineControl.PivotSample> m_DstPivotList = new List<TimelineControl.PivotSample>();

        [MenuItem("Timeline/TestTimelineControl")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TestTimelineControl));
        }

        private void OnEnable()
        {
            if (m_TimelineControl == null)
            {
                m_TimelineControl = new TimelineControl();
            }

        }

        Rect viewInWindow
        {
            get
            {
                return new Rect(0, 0, position.width, position.height);
            }
        }

        void OnGUI()
        {
            // get local durations
            float srcStateDuration = (m_LeftStateTimeB - m_LeftStateTimeA) / (m_LeftStateWeightB - m_LeftStateWeightA);
            float dstStateDuration = (m_RightStateTimeB - m_RightStateTimeA) / (m_RightStateWeightB - m_RightStateWeightA);
            float transitionDuration =2;

            // Set the timeline values
            m_TimelineControl.SrcStartTime = 0f;
            m_TimelineControl.SrcStopTime = srcStateDuration;
            m_TimelineControl.SrcName = "Test";
            m_TimelineControl.HasExitTime = true;

            m_TimelineControl.srcLoop =  false;
            m_TimelineControl.dstLoop = true;

            m_TimelineControl.TransitionStartTime = 0;
            m_TimelineControl.TransitionStopTime =1;

            m_TimelineControl.Time = 0;

            m_TimelineControl.DstStartTime = 1;
            m_TimelineControl.DstStopTime = 2;

            m_TimelineControl.SampleStopTime = 3;

            if (m_TimelineControl.TransitionStopTime == Mathf.Infinity)
                m_TimelineControl.TransitionStopTime = Mathf.Min(m_TimelineControl.DstStopTime, m_TimelineControl.SrcStopTime);


            m_TimelineControl.DstName = "BBB";

            m_TimelineControl.SrcPivotList = m_SrcPivotList;
            m_TimelineControl.DstPivotList = m_DstPivotList;

            // Do the timeline
            Rect previewRect = EditorGUILayout.GetControlRect(false, 150, EditorStyles.label);
            bool changedData = m_TimelineControl.DoTimeline(previewRect);
        }
    }
}