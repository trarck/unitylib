using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH.UI
{
    /// <summary>
    /// 作用相当于mvc中的Controller。
    /// </summary>
    public class UIPanel : MonoBehaviour
    {
        [SerializeField]
        protected int m_Depth=0;

        bool m_Visible = false;

        public virtual void Init(object data)
        {

        }

        public virtual void Show()
        {
            m_Visible = true;
            gameObject.SetActive(true);
            OnShow();
        }

        public virtual void Hide()
        {
            m_Visible = false;
            gameObject.SetActive(false);
            OnHide();
        }

        public virtual bool isVisible { get { return m_Visible; } }

        public virtual void Close()
        {
            if (transform.parent != null)
            {
                Object.Destroy(gameObject);
            }
        }

        public virtual void Clear()
        {

        }

        public virtual void OnShow()
        {

        }

        public virtual void OnHide()
        {

        }

        protected virtual void BindEvents()
        {

        }

        public int depth
        {
            get { return m_Depth; }
            set { m_Depth = value; }
        }

        public string path { get; set; }

       
    }
}