using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using YHEditor;

namespace YHEditor.Timeline
{
    public class TestZoomAreaEditor : EditorWindow
    {
        ZoomableArea m_ZoomArea;

        [MenuItem("Timeline/TestZoomArea")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TestZoomAreaEditor));
        }

        private void OnEnable()
        {
            m_ZoomArea = new ZoomableArea(false, true) { uniformScale = true, upDirection = ZoomableArea.YDirection.Negative };

           //m_ZoomArea.hRangeLocked = false;
           //m_ZoomArea.vRangeLocked = false;
           //m_ZoomArea.hSlider = true;
           //m_ZoomArea.vSlider = true;
           //m_ZoomArea.vAllowExceedBaseRangeMax = false;
           //m_ZoomArea.vAllowExceedBaseRangeMin = false;
           //m_ZoomArea.hBaseRangeMin = 0;
           //m_ZoomArea.vBaseRangeMin = 0;
           //m_ZoomArea.vScaleMax = 1f;
           //m_ZoomArea.vScaleMin = 1f;
           //m_ZoomArea.scaleWithWindow = true;
           //m_ZoomArea.margin = 10;
           //m_ZoomArea.topmargin = 0;
           //m_ZoomArea.bottommargin = 0;
           //m_ZoomArea.upDirection = ZoomableArea.YDirection.Negative;
           // m_ZoomArea.vZoomLockedByDefault = true;

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

            m_ZoomArea.rect = new Rect(0, 0, 600, 600);
            m_ZoomArea.BeginViewGUI();
            EditorGUILayout.TextField("dddd");
            Rect r = new Rect(10, 10, 100, 100);
            r.position = m_ZoomArea.DrawingToViewTransformPoint(r.position);
            r.size = m_ZoomArea.DrawingToViewTransformVector(r.size);
            GUI.Box(r, "test");
            m_ZoomArea.EndViewGUI();
        }
    }
}