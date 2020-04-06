using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using YHEditor;

namespace YHEditor.Timeline
{
    public class TestTimeline : EditorWindow
    {
        private TimeArea m_TimeArea;

        private float m_VerticalScrollBarSize;
        private float m_HorizontalScrollBarSize;

        private float m_LastFrameRate;
        private bool m_TimeAreaDirty = true;

        [MenuItem("Timeline/TestTimeline")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TestTimeline));
        }

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            if (m_TimeArea == null)
            {
                m_TimeArea = new TimeArea(minimalGUI: false)
                {
                    hRangeLocked = false,
                    vRangeLocked = true,
                    margin = 10f,
                    scaleWithWindow = true,
                    hSlider = true,
                    vSlider = false,
                    hBaseRangeMin = 0f,
                    hBaseRangeMax = 9000000.0f,
                    hRangeMin = 0f,
                    hScaleMax = 90000f,
                    rect = timeAreaRect
                };
                m_TimeAreaDirty = true;
                InitTimeAreaFrameRate();
                SyncTimeAreaShownRange();
            }
        }

        private void InitTimeAreaFrameRate()
        {
            m_LastFrameRate = 30;
            m_TimeArea.hTicks.SetTickModulosForFrameRate(m_LastFrameRate);
        }

        private void SyncTimeAreaShownRange()
        {
            Vector2 timeAreaShownRange = new Vector2(-5f, 5f);
            if (!Mathf.Approximately(timeAreaShownRange.x, m_TimeArea.shownArea.x) || !Mathf.Approximately(timeAreaShownRange.y, m_TimeArea.shownArea.xMax))
            {
                if (m_TimeAreaDirty)
                {
                    m_TimeArea.SetShownHRange(timeAreaShownRange.x, timeAreaShownRange.y);
                    m_TimeAreaDirty = false;
                }
  
            }
            m_TimeArea.hBaseRangeMax =10;
        }

        Rect viewInWindow
        {
            get
            {
                return new Rect(0, 0, position.width, position.height);
            }
        }
        public Rect sequenceContentRect => new Rect(350f, 41f, base.position.width - 350f, base.position.height - 41f - m_HorizontalScrollBarSize);

        public Rect timeAreaRect
        {
            get
            {
                return new Rect(sequenceContentRect.x, 19f, Mathf.Max(sequenceContentRect.width, 50f), 22f);
            }
        }



        private void UpdateGUIConstants()
        {
            m_HorizontalScrollBarSize = GUI.skin.horizontalScrollbar.fixedHeight + (float)GUI.skin.horizontalScrollbar.margin.top;
            m_VerticalScrollBarSize =  GUI.skin.verticalScrollbar.fixedWidth + (float)GUI.skin.verticalScrollbar.margin.left;
        }

        void OnGUI()
        {
            UpdateGUIConstants();

            m_TimeArea.rect = new Rect(timeAreaRect.x, timeAreaRect.y, timeAreaRect.width, position.height - timeAreaRect.y);
            if (m_LastFrameRate != 30)
            {
                InitTimeAreaFrameRate();
            }
            //SyncTimeAreaShownRange();
            m_TimeArea.BeginViewGUI();
            m_TimeArea.TimeRuler(timeAreaRect, 30);
            m_TimeArea.DrawMajorTicks(sequenceContentRect, 30);
            m_TimeArea.EndViewGUI();
        }
    }
}