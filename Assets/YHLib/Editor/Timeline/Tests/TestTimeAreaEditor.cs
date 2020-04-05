using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using YHEditor;

namespace YHEditor.Timeline
{
    public class TestTimeAreaEditor : EditorWindow
    {
        private TimeArea m_TimeArea;
        private float m_Time = Mathf.Infinity;

        private float m_StartTime = 0;
        private float m_StopTime = 1;

        private string m_SrcName = "Left";
        private string m_DstName = "Right";

        private bool m_SrcLoop = false;
        private bool m_DstLoop = false;

        private float m_SrcStartTime = 0;
        private float m_SrcStopTime = 0.75f;

        private float m_DstStartTime = 0.25f;
        private float m_DstStopTime = 1;

        private bool m_HasExitTime = false;
        private float m_TransitionStartTime = Mathf.Infinity;
        private float m_TransitionStopTime = Mathf.Infinity;

        private float m_SampleStopTime = Mathf.Infinity;


        private float m_DstDragOffset = 0f;
        private float m_LeftThumbOffset = 0f;
        private float m_RightThumbOffset = 0f;

        private float m_TimeStartDrag = 0f;
        private float m_TimeOffset = 0f;

        private enum DragStates { None, LeftSelection, RightSelection, FullSelection, Destination, Source, Playhead, TimeArea }
        private DragStates m_DragState = DragStates.None;

        private int id = -1;
        private Rect m_Rect = new Rect(0, 0, 0, 0);

        private Vector3[] m_SrcPivotVectors;
        private Vector3[] m_DstPivotVectors;

        private List<PivotSample> m_SrcPivotList = new List<PivotSample>();
        private List<PivotSample> m_DstPivotList = new List<PivotSample>();

        public List<PivotSample> SrcPivotList
        {
            get { return m_SrcPivotList; }
            set { m_SrcPivotList = value; m_SrcPivotVectors = null; }
        }

        public List<PivotSample> DstPivotList
        {
            get { return m_DstPivotList; }
            set { m_DstPivotList = value; m_DstPivotVectors = null; }
        }


        class Styles
        {
            public readonly GUIStyle block = "MeTransitionBlock";
            public readonly GUIStyle leftBlock = "MeTransitionBlock";
            public readonly GUIStyle rightBlock = "MeTransitionBlock";
            public readonly GUIStyle timeBlockRight = "MeTimeBlockRight";
            public readonly GUIStyle timeBlockLeft = "MeTimeBlockLeft";

            public readonly GUIStyle offLeft = "MeTransOffLeft";
            public readonly GUIStyle offRight = "MeTransOffRight";
            public readonly GUIStyle onLeft = "MeTransOnLeft";
            public readonly GUIStyle onRight = "MeTransOnRight";
            public readonly GUIStyle offOn = "MeTransOff2On";
            public readonly GUIStyle onOff = "MeTransOn2Off";
            public readonly GUIStyle background = "MeTransitionBack";
            public readonly GUIStyle header = "MeTransitionHead";

            public readonly GUIStyle handLeft = "MeTransitionHandleLeft";
            public readonly GUIStyle handRight = "MeTransitionHandleRight";
            public readonly GUIStyle handLeftPrev = "MeTransitionHandleLeftPrev";

            public readonly GUIStyle playhead = "MeTransPlayhead";

            public readonly GUIStyle selectHead = "MeTransitionSelectHead";
            public readonly GUIStyle select = "MeTransitionSelect";
        }
        private Styles styles;

        public class PivotSample
        {
            public float m_Time;
            public float m_Weight;
        }


        public bool srcLoop
        {
            get { return m_SrcLoop; }
            set { m_SrcLoop = value; }
        }

        public bool dstLoop
        {
            get { return m_DstLoop; }
            set { m_DstLoop = value; }
        }


        public float Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }

        public float StartTime
        {
            get { return m_StartTime; }
            set { m_StartTime = value; }
        }

        public float StopTime
        {
            get { return m_StopTime; }
            set { m_StopTime = value; }
        }

        public string SrcName
        {
            get { return m_SrcName; }
            set { m_SrcName = value; }
        }

        public string DstName
        {
            get { return m_DstName; }
            set { m_DstName = value; }
        }

        public float SrcStartTime
        {
            get { return m_SrcStartTime; }
            set { m_SrcStartTime = value; }
        }

        public float SrcStopTime
        {
            get { return m_SrcStopTime; }
            set { m_SrcStopTime = value; }
        }

        public float SrcDuration
        {
            get { return SrcStopTime - SrcStartTime; }
        }

        public float DstStartTime
        {
            get { return m_DstStartTime; }
            set { m_DstStartTime = value; }
        }

        public float DstStopTime
        {
            get { return m_DstStopTime; }
            set { m_DstStopTime = value; }
        }

        public float DstDuration
        {
            get { return DstStopTime - DstStartTime; }
        }

        public float TransitionStartTime
        {
            get { return m_TransitionStartTime; }
            set { m_TransitionStartTime = value; }
        }

        public float TransitionStopTime
        {
            get { return m_TransitionStopTime; }
            set { m_TransitionStopTime = value; }
        }

        public bool HasExitTime
        {
            get { return m_HasExitTime; }
            set { m_HasExitTime = value; }
        }

        public float TransitionDuration
        {
            get { return TransitionStopTime - TransitionStartTime; }
        }

        public float SampleStopTime
        {
            get { return m_SampleStopTime; }
            set { m_SampleStopTime = value; }
        }


        public void ResetRange()
        {
            m_TimeArea.SetShownHRangeInsideMargins(0, StopTime);
        }


        [MenuItem("Timeline/TestTimeAreaEditor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TestTimeAreaEditor));
        }

        private void Init()
        {
            if (id == -1)
                id = 100;// GUIUtility.GetPermanentControlID();

            if (m_TimeArea == null)
            {
                m_TimeArea = new TimeArea(false);
                m_TimeArea.hRangeLocked = false;
                m_TimeArea.vRangeLocked = true;
                m_TimeArea.hSlider = false;
                m_TimeArea.vSlider = false;
                m_TimeArea.margin = 10;
                m_TimeArea.scaleWithWindow = true;
                m_TimeArea.hTicks.SetTickModulosForFrameRate(30);
            }

            if (styles == null)
                styles = new Styles();
        }

        private void OnEnable()
        {
            Init();
        }

        Rect viewInWindow
        {
            get
            {
                return new Rect(0, 0, position.width, position.height);
            }
        }

        internal static float DiscardLeastSignificantDecimal(float v)
        {
            int decimals = Mathf.Clamp((int)(5 - Mathf.Log10(Mathf.Abs(v))), 0, 15);
            return (float)System.Math.Round(v, decimals, System.MidpointRounding.AwayFromZero);
        }

        internal static int GetNumberOfDecimalsForMinimumDifference(float minDifference)
        {
            return Mathf.Clamp(-Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(minDifference))), 0, 15);
        }

        internal static float RoundBasedOnMinimumDifference(float valueToRound, float minDifference)
        {
            if (minDifference == 0)
                return DiscardLeastSignificantDecimal(valueToRound);
            return (float)System.Math.Round(valueToRound, GetNumberOfDecimalsForMinimumDifference(minDifference), System.MidpointRounding.AwayFromZero);
        }
        
        void OnGUI()
        {
            bool hasModifiedData = false;

            // Time line
            Rect timeRect =   EditorGUILayout.GetControlRect(false, 150, EditorStyles.label); ;
            float timeAreaStart = m_TimeArea.PixelToTime(timeRect.xMin, timeRect);
            float timeAreaStop = m_TimeArea.PixelToTime(timeRect.xMax, timeRect);
            if (!Mathf.Approximately(timeAreaStart, StartTime))
            {
                StartTime = timeAreaStart;
                GUI.changed = true;
            }
            if (!Mathf.Approximately(timeAreaStop, StopTime))
            {
                StopTime = timeAreaStop;
                GUI.changed = true;
            }

            Time = Mathf.Max(Time, 0f);

            if (Event.current.type == EventType.Repaint)
                m_TimeArea.rect = timeRect;

            m_TimeArea.BeginViewGUI();
            m_TimeArea.EndViewGUI();

            GUI.BeginGroup(timeRect);
            {
                Event evt = Event.current;
                Rect r = new Rect(0, 0, timeRect.width, timeRect.height);
                Rect headerRect = new Rect(0, 0, timeRect.width, 18);
                Rect bodyRect = new Rect(0, 18, timeRect.width, 132);

                // get the relevant positions in pixels
                float srcStart = m_TimeArea.TimeToPixel(SrcStartTime, r);
                float srcStop = m_TimeArea.TimeToPixel(SrcStopTime, r);
                float dstStart = m_TimeArea.TimeToPixel(DstStartTime, r) + m_DstDragOffset;
                float dstStop = m_TimeArea.TimeToPixel(DstStopTime, r) + m_DstDragOffset;
                float transStart = m_TimeArea.TimeToPixel(TransitionStartTime, r) + m_LeftThumbOffset;
                float transStop = m_TimeArea.TimeToPixel(TransitionStopTime, r) + m_RightThumbOffset;
                float playPoint = m_TimeArea.TimeToPixel(Time, r);

                // get the relevant Rects

                Rect srcRect = new Rect(srcStart, 85, srcStop - srcStart, 32);
                Rect dstRect = new Rect(dstStart, 117, dstStop - dstStart, 32);
                Rect transHeaderRect = new Rect(transStart, 0, transStop - transStart, 18);
                Rect transRect = new Rect(transStart, 18, transStop - transStart, r.height - 18);
                Rect leftThumbRect = new Rect(transStart - 9, 5, 9, 15);
                Rect rightThumbRect = new Rect(transStop, 5, 9, 15);
                Rect playHeadRect = new Rect(playPoint - 7, 4, 15, 15);


                // handle keyboard
                if (evt.type == EventType.KeyDown)
                {
                    if (EditorGUIUtility.keyboardControl == id)
                        if (m_DragState == DragStates.Destination)
                            m_DstDragOffset = 0f;
                    if (m_DragState == DragStates.LeftSelection)
                        m_LeftThumbOffset = 0f;
                    if (m_DragState == DragStates.RightSelection)
                        m_RightThumbOffset = 0f;
                    if (m_DragState == DragStates.Playhead)
                        m_TimeOffset = 0f;
                    if (m_DragState == DragStates.FullSelection)
                    {
                        m_LeftThumbOffset = 0f;
                        m_RightThumbOffset = 0f;
                    }
                }
                
                // handle mouse down
                if (evt.type == EventType.MouseDown)
                {
                    if (r.Contains(evt.mousePosition))
                    {
                        EditorGUIUtility.hotControl = id;
                        EditorGUIUtility.keyboardControl = id;
                        if (playHeadRect.Contains(evt.mousePosition))
                        {
                            m_DragState = DragStates.Playhead;
                            m_TimeStartDrag = m_TimeArea.TimeToPixel(Time, r);
                        }
                        else if (srcRect.Contains(evt.mousePosition))
                            m_DragState = DragStates.Source;
                        else if (dstRect.Contains(evt.mousePosition))
                            m_DragState = DragStates.Destination;
                        else if (leftThumbRect.Contains(evt.mousePosition))
                            m_DragState = DragStates.LeftSelection;
                        else if (rightThumbRect.Contains(evt.mousePosition))
                            m_DragState = DragStates.RightSelection;
                        else if (transHeaderRect.Contains(evt.mousePosition))
                            m_DragState = DragStates.FullSelection;
                        else if (headerRect.Contains(evt.mousePosition))
                            m_DragState = DragStates.TimeArea;
                        else if (bodyRect.Contains(evt.mousePosition))
                            m_DragState = DragStates.TimeArea;
                        else
                            m_DragState = DragStates.None;
                        evt.Use();
                    }
                }

                // handle mouse drag
                if (evt.type == EventType.MouseDrag)
                {
                    if (EditorGUIUtility.hotControl == id)
                    {
                        switch (m_DragState)
                        {
                            case DragStates.Source:
                            case DragStates.TimeArea:
                                m_TimeArea.m_Translation.x += evt.delta.x;
                                break;
                            case DragStates.Destination:
                                m_DstDragOffset += evt.delta.x;
                                EnforceConstraints();
                                break;
                            case DragStates.LeftSelection:
                                // clamp the delta when off range
                                if ((evt.delta.x > 0 && evt.mousePosition.x > srcStart) ||
                                    (evt.delta.x < 0 && evt.mousePosition.x < transStop))
                                    m_LeftThumbOffset += evt.delta.x;
                                EnforceConstraints();
                                break;
                            case DragStates.RightSelection:
                                // clamp the delta when off range
                                if (evt.delta.x > 0 && evt.mousePosition.x > transStart || evt.delta.x < 0)
                                    m_RightThumbOffset += evt.delta.x;
                                EnforceConstraints();
                                break;
                            case DragStates.FullSelection:
                                m_RightThumbOffset += evt.delta.x;
                                m_LeftThumbOffset += evt.delta.x;
                                EnforceConstraints();
                                break;
                            case DragStates.Playhead:
                                if ((evt.delta.x > 0 && evt.mousePosition.x > srcStart) ||
                                    (evt.delta.x < 0 && evt.mousePosition.x <= m_TimeArea.TimeToPixel(SampleStopTime, r)))
                                    m_TimeOffset += evt.delta.x;
                                Time = m_TimeArea.PixelToTime(m_TimeStartDrag + m_TimeOffset, r);

                                break;
                            case DragStates.None:
                                break;
                        }
                        evt.Use();
                        GUI.changed = true;
                    }
                }
                
                
                // handle mouse up both when it happens over the control area and outside control area (case 834214)
                if (Event.current.GetTypeForControl(id) == EventType.MouseUp)
                {
                    SrcStartTime = m_TimeArea.PixelToTime(srcStart, r);
                    SrcStopTime = m_TimeArea.PixelToTime(srcStop, r);
                    DstStartTime = m_TimeArea.PixelToTime(dstStart, r);
                    DstStopTime = m_TimeArea.PixelToTime(dstStop, r);
                    TransitionStartTime = m_TimeArea.PixelToTime(transStart, r);
                    TransitionStopTime = m_TimeArea.PixelToTime(transStop, r);
                    GUI.changed = true;
                    m_DragState = DragStates.None;

                    hasModifiedData = WasDraggingData();
                    m_LeftThumbOffset = 0f;
                    m_RightThumbOffset = 0f;
                    m_TimeOffset = 0f;
                    m_DstDragOffset = 0f;
                    EditorGUIUtility.hotControl = 0;
                    evt.Use();
                }
                

                // draw the background boxes
                GUI.Box(headerRect, GUIContent.none, styles.header);
                GUI.Box(bodyRect, GUIContent.none, styles.background);

                // draw ticks and curves on top of background boxes
                m_TimeArea.DrawMajorTicks(bodyRect, 30);


                GUIContent srcContent = new GUIContent(SrcName);

                // draw src Loop
                int srcLoopCount = srcLoop ? (1 + (int)((transStop - srcRect.xMin) / (srcRect.xMax - srcRect.xMin))) : 1;
                Rect loopRect = srcRect;
                if (srcRect.width < 10) // if smaller than 10 pixel, group
                {
                    loopRect = new Rect(srcRect.x, srcRect.y, (srcRect.xMax - srcRect.xMin) * srcLoopCount, srcRect.height);
                    srcLoopCount = 1;
                }

                for (int loopSrcIt = 0; loopSrcIt < srcLoopCount; loopSrcIt++)
                {
                    GUI.BeginGroup(loopRect, GUIContent.none, styles.leftBlock);
                    float widthBefore = transStart - loopRect.xMin;
                    float widthDuring = transStop - transStart;
                    float widthAfter = (loopRect.xMax - loopRect.xMin) - (widthBefore + widthDuring);
                    if (widthBefore > 0)
                        GUI.Box(new Rect(0, 0, widthBefore, srcRect.height), GUIContent.none, styles.onLeft);
                    if (widthDuring > 0)
                        GUI.Box(new Rect(widthBefore, 0, widthDuring, srcRect.height), GUIContent.none, styles.onOff);
                    if (widthAfter > 0)
                        GUI.Box(new Rect(widthBefore + widthDuring, 0, widthAfter, srcRect.height), GUIContent.none, styles.offRight);
                    float srcAlphaTarget = 1f;
                    float srcLabelRight = styles.block.CalcSize(srcContent).x;
                    float srcPercentLeft = Mathf.Max(0, widthBefore) - 20;
                    float srcPercentRight = srcPercentLeft + 15;
                    if (srcPercentLeft < srcLabelRight && srcPercentRight > 0f && m_DragState == DragStates.LeftSelection)
                        srcAlphaTarget = 0f;
                    GUI.EndGroup();

                    float srcAlpha = styles.leftBlock.normal.textColor.a;
                    if (!Mathf.Approximately(srcAlpha, srcAlphaTarget) && Event.current.type == EventType.Repaint)
                    {
                        srcAlpha = Mathf.Lerp(srcAlpha, srcAlphaTarget, 0.1f);
                        styles.leftBlock.normal.textColor = new Color(styles.leftBlock.normal.textColor.r, styles.leftBlock.normal.textColor.g, styles.leftBlock.normal.textColor.b, srcAlpha);
                        HandleUtility.Repaint();
                    }
                    GUI.Box(loopRect, srcContent, styles.leftBlock);
                    loopRect = new Rect(loopRect.xMax, 85, loopRect.xMax - loopRect.xMin, 32);
                }


                GUIContent dstContent = new GUIContent(DstName);
                int dstLoopCount = dstLoop ? (1 + (int)((transStop - dstRect.xMin) / (dstRect.xMax - dstRect.xMin))) : 1;
                loopRect = dstRect;
                if (dstRect.width < 10) // if smaller than 10 pixel, group
                {
                    loopRect = new Rect(dstRect.x, dstRect.y, (dstRect.xMax - dstRect.xMin) * dstLoopCount, dstRect.height);
                    dstLoopCount = 1;
                }

                for (int loopDstIt = 0; loopDstIt < dstLoopCount; loopDstIt++)
                {
                    // draw the DST box
                    GUI.BeginGroup(loopRect, GUIContent.none, styles.rightBlock);
                    float widthBefore = transStart - loopRect.xMin;
                    float widthDuring = transStop - transStart;
                    float widthAfter = (loopRect.xMax - loopRect.xMin) - (widthBefore + widthDuring);
                    if (widthBefore > 0)
                        GUI.Box(new Rect(0, 0, widthBefore, dstRect.height), GUIContent.none, styles.offLeft);
                    if (widthDuring > 0)
                        GUI.Box(new Rect(widthBefore, 0, widthDuring, dstRect.height), GUIContent.none, styles.offOn);
                    if (widthAfter > 0)
                        GUI.Box(new Rect(widthBefore + widthDuring, 0, widthAfter, dstRect.height), GUIContent.none, styles.onRight);
                    float dstAlphaTarget = 1f;
                    float dstLabelRight = styles.block.CalcSize(dstContent).x;
                    float dstPercentLeft = Mathf.Max(0, widthBefore) - 20;
                    float dstPercentRight = dstPercentLeft + 15;
                    if (dstPercentLeft < dstLabelRight && dstPercentRight > 0f && (m_DragState == DragStates.LeftSelection || m_DragState == DragStates.Destination))
                        dstAlphaTarget = 0f;
                    GUI.EndGroup();
                    float dstAlpha = styles.rightBlock.normal.textColor.a;
                    if (!Mathf.Approximately(dstAlpha, dstAlphaTarget) && Event.current.type == EventType.Repaint)
                    {
                        dstAlpha = Mathf.Lerp(dstAlpha, dstAlphaTarget, 0.1f);
                        styles.rightBlock.normal.textColor = new Color(styles.rightBlock.normal.textColor.r, styles.rightBlock.normal.textColor.g, styles.rightBlock.normal.textColor.b, dstAlpha);
                        HandleUtility.Repaint();
                    }
                    GUI.Box(loopRect, dstContent, styles.rightBlock);

                    loopRect = new Rect(loopRect.xMax, loopRect.yMin, loopRect.xMax - loopRect.xMin, 32);
                }

                // draw the transition selection box in the body
                GUI.Box(transRect, GUIContent.none, styles.select);

                // draw the transition selection box in the header
                GUI.Box(transHeaderRect, GUIContent.none, styles.selectHead);

                m_TimeArea.TimeRuler(headerRect, 30);

                // draw the thumbs
                GUI.Box(leftThumbRect, GUIContent.none, (m_HasExitTime) ? styles.handLeft : styles.handLeftPrev);
                GUI.Box(rightThumbRect, GUIContent.none, styles.handRight);

                // playhead and bar
                GUI.Box(playHeadRect, GUIContent.none, styles.playhead);
                Color oldColor = Handles.color;
                Handles.color = Color.white;
                Handles.DrawLine(new Vector3(playPoint, 19, 0), new Vector3(playPoint, r.height, 0));
                Handles.color = oldColor;


                bool oneFrameSrc = (SrcStopTime - SrcStartTime) < 1.0f / 30.0f;
                bool oneFrameDst = (DstStopTime - DstStartTime) < 1.0f / 30.0f;
                // show normalized time label when moving destination state
                if (m_DragState == DragStates.Destination && !oneFrameDst)
                {
                    Rect transLabelRect = new Rect(transStart - 50, dstRect.y, 45, dstRect.height);
                    string transLabel = string.Format("{0:0%}", (transStart - dstStart) / (dstStop - dstStart));
                    GUI.Box(transLabelRect, new GUIContent(transLabel), styles.timeBlockRight);
                }

                // show normalized time label on left side when moving left edge of transition
                if (m_DragState == DragStates.LeftSelection)
                {
                    if (!oneFrameSrc)
                    {
                        Rect srcLabelRect = new Rect(transStart - 50, srcRect.y, 45, srcRect.height);
                        string srcLabel = string.Format("{0:0%}", (transStart - srcStart) / (srcStop - srcStart));
                        GUI.Box(srcLabelRect, new GUIContent(srcLabel), styles.timeBlockRight);
                    }

                    if (!oneFrameDst)
                    {
                        Rect dstLabelRect = new Rect(transStart - 50, dstRect.y, 45, dstRect.height);
                        string dstLabel = string.Format("{0:0%}", (transStart - dstStart) / (dstStop - dstStart));
                        GUI.Box(dstLabelRect, new GUIContent(dstLabel), styles.timeBlockRight);
                    }
                }

                // show normalized time label on right side when moving right edge of transition
                if (m_DragState == DragStates.RightSelection)
                {
                    if (!oneFrameSrc)
                    {
                        Rect srcLabelRect = new Rect(transStop + 5, srcRect.y, 45, srcRect.height);
                        string srcLabel = string.Format("{0:0%}", (transStop - srcStart) / (srcStop - srcStart));
                        GUI.Box(srcLabelRect, new GUIContent(srcLabel), styles.timeBlockLeft);
                    }

                    if (!oneFrameDst)
                    {
                        Rect dstLabelRect = new Rect(transStop + 5, dstRect.y, 45, dstRect.height);
                        string dstLabel = string.Format("{0:0%}", (transStop - dstStart) / (dstStop - dstStart));
                        GUI.Box(dstLabelRect, new GUIContent(dstLabel), styles.timeBlockLeft);
                    }
                }

                //DoPivotCurves();
            }
            GUI.EndGroup();

        }
        private bool WasDraggingData()
        {
            return m_DstDragOffset != 0 ||
                m_LeftThumbOffset != 0 ||
                m_RightThumbOffset != 0;
        }

        private void EnforceConstraints()
        {
            Rect r = new Rect(0, 0, m_Rect.width, 150);
            if (m_DragState == DragStates.LeftSelection)
            {
                float minLimit = m_TimeArea.TimeToPixel(SrcStartTime, r) - m_TimeArea.TimeToPixel(TransitionStartTime, r);
                float maxLimit = m_TimeArea.TimeToPixel(TransitionStopTime, r) - m_TimeArea.TimeToPixel(TransitionStartTime, r);

                m_LeftThumbOffset = Mathf.Clamp(m_LeftThumbOffset, minLimit, maxLimit);
            }

            if (m_DragState == DragStates.RightSelection)
            {
                float minLimit = m_TimeArea.TimeToPixel(TransitionStartTime, r) - m_TimeArea.TimeToPixel(TransitionStopTime, r);
                if (m_RightThumbOffset < minLimit)
                    m_RightThumbOffset = minLimit;
            }
        }
    }
}