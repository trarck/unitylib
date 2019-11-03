using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YH.UI
{
    [AddComponentMenu("UI/BoxSlider", 35)]
    [RequireComponent(typeof(RectTransform))]
    public class BoxSlider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        /// <summary>
        /// Setting that indicates one of four directions.
        /// </summary>
        public enum Origin
        {
            /// <summary>
            /// From the left to the right
            /// </summary>
            LeftBottom,

            /// <summary>
            /// From the right to the left
            /// </summary>
            RightBottom,

            /// <summary>
            /// From the bottom to the top.
            /// </summary>
            LeftTop,

            /// <summary>
            /// From the top to the bottom.
            /// </summary>
            RightTop,
        }

        [Serializable]
        /// <summary>
        /// Event type used by the UI.Slider.
        /// </summary>
        public class SliderEvent : UnityEvent<Vector2> { }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }

        [SerializeField]
        private RectTransform m_HandleRect;

        /// <summary>
        /// Optional RectTransform to use as a handle for the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///     //Reference to new "RectTransform" (Child of "Handle Slide Area").
        ///     public RectTransform handleHighlighted;
        ///
        ///     //Deactivates the old Handle, then assigns and enables the new one.
        ///     void Start()
        ///     {
        ///         mainSlider.handleRect.gameObject.SetActive(false);
        ///         mainSlider.handleRect = handleHighlighted;
        ///         mainSlider.handleRect.gameObject.SetActive(true);
        ///     }
        /// }
        /// </code>
        /// </example>
        public RectTransform handleRect { get { return m_HandleRect; } set { if (SetClass(ref m_HandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [Space]

        [SerializeField]
        private Origin m_Origin = Origin.LeftBottom;

        public Origin direction { get { return m_Origin; } set { if (SetStruct(ref m_Origin, value)) UpdateVisuals(); } }

        [SerializeField]
        private Vector2 m_MinValue = Vector2.zero;

        public Vector2 minValue { get { return m_MinValue; } set { if (SetStruct(ref m_MinValue, value)) { Set(m_Value); UpdateVisuals(); } } }

        [SerializeField]
        private Vector2 m_MaxValue = Vector2.one;

        public Vector2 maxValue { get { return m_MaxValue; } set { if (SetStruct(ref m_MaxValue, value)) { Set(m_Value); UpdateVisuals(); } } }

        [SerializeField]
        private bool m_WholeNumbers = false;

        public bool wholeNumbers { get { return m_WholeNumbers; } set { if (SetStruct(ref m_WholeNumbers, value)) { Set(m_Value); UpdateVisuals(); } } }

        [SerializeField]
        protected Vector2 m_Value;
        public virtual Vector2 value
        {
            get
            {
                if (wholeNumbers)
                {
                    return new Vector2(Mathf.Round(m_Value.x), Mathf.Round(m_Value.y));
                }
                return m_Value;
            }
            set
            {
                Set(value);
            }
        }

        public Vector2 normalizedValue
        {
            get
            {
                Vector2 v = new Vector2();

                if (Mathf.Approximately(minValue.x, maxValue.x))
                {
                    v.x = 0;
                }
                else
                {
                    v.x = Mathf.InverseLerp(minValue.x, maxValue.x, value.x);
                }

                if (Mathf.Approximately(minValue.y, maxValue.y))
                {
                    v.y = 0;
                }
                else
                {
                    v.y = Mathf.InverseLerp(minValue.y, maxValue.y, value.y);
                }

                return v;
            }
            set
            {
                this.value = new Vector2(
                    Mathf.Lerp(minValue.x, maxValue.x, value.x), 
                    Mathf.Lerp(minValue.y, maxValue.y, value.y)
                );
            }
        }

        [Space]

        [SerializeField]
        private SliderEvent m_OnValueChanged = new SliderEvent();

        public SliderEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        // Private fields
        private Transform m_HandleTransform;
        private RectTransform m_HandleContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        // Size of each step.
        Vector2 stepSize { get { return wholeNumbers ? Vector2.one : (maxValue - minValue) * 0.1f; } }

        protected BoxSlider()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (wholeNumbers)
            {
                m_MinValue.x = Mathf.Round(m_MinValue.x);
                m_MinValue.y = Mathf.Round(m_MinValue.y);
                m_MaxValue.x = Mathf.Round(m_MaxValue.x);
                m_MaxValue.y = Mathf.Round(m_MaxValue.y);
            }

            //Onvalidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
            if (IsActive())
            {
                UpdateCachedReferences();
                Set(m_Value, false);
                // Update rects since other things might affect them even if value didn't change.
                UpdateVisuals();
            }

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(value);
#endif
        }

        /// <summary>
        /// See ICanvasElement.LayoutComplete
        /// </summary>
        public virtual void LayoutComplete()
        { }

        /// <summary>
        /// See ICanvasElement.GraphicUpdateComplete
        /// </summary>
        public virtual void GraphicUpdateComplete()
        { }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            Set(m_Value, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Has value changed? Various elements of the slider have the old normalisedValue assigned, we can use this to perform a comparison.
            // We also need to ensure the value stays within min/max.
            m_Value = ClampValue(m_Value);
            Vector2 oldNormalizedValue = normalizedValue;
            if (m_HandleContainerRect != null)
            {
                oldNormalizedValue.x = (reverseValueX ? 1 - m_HandleRect.anchorMin[0] : m_HandleRect.anchorMin[0]);
                oldNormalizedValue.y = (reverseValueY ? 1 - m_HandleRect.anchorMin[1] : m_HandleRect.anchorMin[1]);
            }
            UpdateVisuals();

            if (oldNormalizedValue != normalizedValue)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
                onValueChanged.Invoke(m_Value);
            }
        }

        void UpdateCachedReferences()
        {
            if (m_HandleRect && m_HandleRect != (RectTransform)transform)
            {
                m_HandleTransform = m_HandleRect.transform;
                if (m_HandleTransform.parent != null)
                    m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_HandleRect = null;
                m_HandleContainerRect = null;
            }
        }

        Vector2 ClampValue(Vector2 input)
        {
            Vector2 newValue = new Vector2(Mathf.Clamp(input.x, minValue.x, maxValue.x), Mathf.Clamp(input.y, minValue.y, maxValue.y));
            if (wholeNumbers)
            {
                newValue.x = Mathf.Round(newValue.x);
                newValue.y = Mathf.Round(newValue.y);
            }
            return newValue;
        }

        /// <summary>
        /// Set the value of the slider.
        /// </summary>
        /// <param name="input">The new value for the slider.</param>
        void Set(Vector2 input)
        {
            Set(input, true);
        }

        /// <summary>
        /// Set the value of the slider.
        /// </summary>
        /// <param name="input">The new value for the slider.</param>
        /// <param name="sendCallback">If the OnValueChanged callback should be invoked.</param>
        /// <remarks>
        /// Process the input to ensure the value is between min and max value. If the input is different set the value and send the callback is required.
        /// </remarks>
        protected virtual void Set(Vector2 input, bool sendCallback)
        {
            // Clamp the input
            Vector2 newValue = ClampValue(input);

            // If the stepped value doesn't match the last one, it's time to update
            if (m_Value == newValue)
                return;

            m_Value = newValue;
            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
                m_OnValueChanged.Invoke(newValue);
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive())
                return;

            UpdateVisuals();
        }

        bool reverseValueX
        {
            get
            {
                return m_Origin == Origin.RightBottom || m_Origin == Origin.RightTop;
            }
        }
        bool reverseValueY
        {
            get
            {
                return m_Origin == Origin.LeftTop || m_Origin == Origin.RightTop;
            }
        }

        // Force-update the slider. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            m_Tracker.Clear();

            if (m_HandleContainerRect != null)
            {
                m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[0] = anchorMax[0] = (reverseValueX ? (1 - normalizedValue.x) : normalizedValue.x);
                anchorMin[1] = anchorMax[1] = (reverseValueY ? (1 - normalizedValue.y) : normalizedValue.y);
                m_HandleRect.anchorMin = anchorMin;
                m_HandleRect.anchorMax = anchorMax;
            }
        }

        // Update the slider's position based on the mouse.
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_HandleContainerRect;
            if (clickRect != null && (clickRect.rect.size.x > 0 || clickRect.rect.size.y > 0))
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                Vector2 val =new Vector2( Mathf.Clamp01((localCursor.x - m_Offset.x) / clickRect.rect.size.x),Mathf.Clamp01((localCursor.y - m_Offset.y)/ clickRect.rect.size.y));
                if (reverseValueX)
                {
                    val.x = 1.0f - val.x;
                }
                if (reverseValueY)
                {
                    val.y = 1.0f - val.y;
                }
                normalizedValue = val;
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            m_Offset = Vector2.zero;
            if (m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.position, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.position, eventData.pressEventCamera, out localMousePos))
                    m_Offset = localMousePos;
            }
            else
            {
                // Outside the slider handle - jump to this point instead
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            Vector2 dir = new Vector2(1,1);

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    dir.x = reverseValueX ? 1 : -1;
                    break;
                case MoveDirection.Right:
                    dir.x = reverseValueX ? -1 : 1;
                    break;
                case MoveDirection.Up:
                    dir.y = reverseValueX ? -1 :1;
                    break;
                case MoveDirection.Down:
                    dir.y = reverseValueX ? 1 : -1;
                    break;
            }

            Set(value + stepSize * dir);
            base.OnMove(eventData);
        }

        ///// <summary>
        ///// See Selectable.FindSelectableOnLeft
        ///// </summary>
        //public override Selectable FindSelectableOnLeft()
        //{
        //    if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
        //        return null;
        //    return base.FindSelectableOnLeft();
        //}

        ///// <summary>
        ///// See Selectable.FindSelectableOnRight
        ///// </summary>
        //public override Selectable FindSelectableOnRight()
        //{
        //    if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
        //        return null;
        //    return base.FindSelectableOnRight();
        //}

        ///// <summary>
        ///// See Selectable.FindSelectableOnUp
        ///// </summary>
        //public override Selectable FindSelectableOnUp()
        //{
        //    if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
        //        return null;
        //    return base.FindSelectableOnUp();
        //}

        ///// <summary>
        ///// See Selectable.FindSelectableOnDown
        ///// </summary>
        //public override Selectable FindSelectableOnDown()
        //{
        //    if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
        //        return null;
        //    return base.FindSelectableOnDown();
        //}

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        /// <summary>
        /// Sets the direction of this slider, optionally changing the layout as well.
        /// </summary>
        /// <param name="direction">The direction of the slider</param>
        /// <param name="includeRectLayouts">Should the layout be flipped together with the slider direction</param>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     public void Start()
        ///     {
        ///         mainSlider.SetDirection(Slider.Direction.LeftToRight, false);
        ///     }
        /// }
        /// </code>
        /// </example>
        public void SetDirection(Origin direction, bool includeRectLayouts)
        {
            bool oldReverseX = reverseValueX;
            bool oldReverseY = reverseValueY;
            this.direction = direction;

            if (!includeRectLayouts)
                return;

            if (reverseValueX != oldReverseX)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, 0, true, true);

            if (reverseValueY != oldReverseY)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, 1, true, true);
        }
    }
}
