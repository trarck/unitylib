using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YH.UI.Mvc
{
    public class View : UIBehaviour, IView
    {
        RectTransform m_RectTransform;

        List<IView> m_Children = new List<IView>();

        IController m_Controller;

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

        public List<IView> children
        {
            get
            {
                return m_Children;
            }
            set
            {
                m_Children = value;
            }
        }

        public IView superView
        {
            get
            {
                if (rectTransorm.parent != null)
                {
                    return rectTransorm.parent.GetComponent<IView>();
                }
                return null;
            }
        }

        public IController controller
        {
            get
            {
                return m_Controller;
            }
            set
            {
                m_Controller = value;
            }
        }
        
        #region SubView
        public void AddSubView(IView view)
        {
            if (m_Controller!=null)
            {
                m_Controller.ViewWillAppear();
            }

            view.rectTransorm.SetParent(rectTransorm, false);
            m_Children.Add(view);
            DidAddSubView(view);

            if (m_Controller != null)
            {
                m_Controller.ViewDidAppear();
            }
        }

        public void InsertSubView(IView view,int index)
        {
            if (m_Controller != null)
            {
                m_Controller.ViewWillAppear();
            }

            view.rectTransorm.SetParent(rectTransorm, false);
            view.rectTransorm.SetSiblingIndex(index);
            m_Children.Insert(index,view);

            DidAddSubView(view);

            if (m_Controller != null)
            {
                m_Controller.ViewDidAppear();
            }
        }

        public void InsertSubView(IView view, IView siblingSubView)
        {
            int index = m_Children.IndexOf(siblingSubView);
            InsertSubView(view, index);
        }

        public void RemoveFromSuperView()
        {
            IView parentView = superView;
            if (parentView != null)
            {
                if (m_Controller != null)
                {
                    m_Controller.ViewWillDisappear();
                }

                rectTransorm.SetParent(null);
                parentView.children.Remove(this);
                parentView.WillRemoveSubView(this);

                if (m_Controller != null)
                {
                    m_Controller.ViewDidDisappear();
                }
            }
        }

        public virtual void CleanSubView()
        {
            foreach (var v in m_Children)
            {
                if (v.controller != null)
                {
                    v.controller.ViewWillDisappear();

                    v.rectTransorm.SetParent(null);

                    v.controller.ViewDidDisappear();
                }
                else
                {
                    v.rectTransorm.SetParent(null);
                }
            }
            m_Children.Clear();
        }

        public void BringSubViewToFront(IView view)
        {
            m_Children.Remove(view);
            m_Children.Add(view);
            view.rectTransorm.SetSiblingIndex(m_Children.Count - 1);
        }

        public virtual void DidAddSubView(IView subView)
        {

        }

        public virtual void WillRemoveSubView(IView subView)
        {

        }
        #endregion

        #region Event
        protected override void Awake()
        {
            base.Awake();
            if (m_Controller != null)
            {
                m_Controller.OnViewAwake();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_Controller != null)
            {
                m_Controller.OnViewDestroy();
            }
            //删除子视图的引用
            m_Children.Clear();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_Controller != null)
            {
                m_Controller.OnViewEnable();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_Controller != null)
            {
                m_Controller.OnViewDisable();
            }
        }
        #endregion

        public virtual void Dispose()
        {
            RemoveFromSuperView();
            Destroy(gameObject);
            controller = null;
        }
    }
}
