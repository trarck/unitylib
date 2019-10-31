using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YH.UI.Mvc
{
    public class View : UIBehaviour, IView
    {
        RectTransform m_RectTransform;

        public RectTransform rectTransorm
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = GetComponent<RectTransform>();
                }
                return m_RectTransform;
            }
        }
    }
}
