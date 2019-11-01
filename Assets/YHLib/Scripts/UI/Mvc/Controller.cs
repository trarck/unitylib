using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YH.UI.Mvc
{
    public class Controller :  IController
    {
        IView m_View;
        string m_ViewAsset;

        public IView view
        {
            get
            {
                return m_View;
            }
            set
            {
                m_View = value;
            }
        }

        //~Controller()
        //{
        //    foreach (var c in m_Children)
        //    {
        //        c.WillMoveToParentController(null);
        //        c.parent = null;
        //        c.DidMoveToParentController(null);
        //    }
        //}

        public virtual void Init()
        {

        }

        public virtual void Init(string viewAsset)
        {
            m_ViewAsset=viewAsset;
        }

        public virtual void Dispose()
        {
            UnloadView();
            foreach(var c in m_Children)
            {
                c.Dispose();
                c.parent = null;
            }
            m_Children = null;
        }

        #region View Operate
        public virtual void LoadView()
        {
            //Load from asset file
            LoadFromAsset();
        }

        protected void LoadFromAsset()
        {
            if (string.IsNullOrEmpty(m_ViewAsset))
            {
                string fullName = GetType().FullName;
                int dotIndx = fullName.LastIndexOf(".");
                m_ViewAsset = fullName.Substring(dotIndx)+".prefab";
            }
            AssetManager.AssetManager.Instance.LoadAsset(m_ViewAsset, OnAssetLoaded);
        }

        void OnAssetLoaded(AssetManager.AssetReference ar)
        {
            GameObject viewPrefab = ar != null ? ar.asset as GameObject : null;
            if (viewPrefab != null)
            {
                GameObject viewObj = GameObject.Instantiate<GameObject>(viewPrefab);
                m_View = viewObj.GetComponent<IView>();
                if (m_View == null)
                {
                    m_View = viewObj.AddComponent<View>();
                }
                m_View.controller = this;
            }
            else
            {
                CreateEmpyView();
            }

            ViewDidLoad();
        }

        protected void CreateEmpyView()
        {
            GameObject viewObj = new GameObject();
            m_View = viewObj.AddComponent<View>();
            m_View.controller = this;
        }

        public virtual void ViewDidLoad()
        {

        }

        public virtual void UnloadView()
        {
            if (m_View!=null)
            {
                m_View.Dispose();
                m_View = null;
            }
        }

        public virtual bool isViewLoaded
        {
            get
            {
                return m_View != null;
            }
        }
        #endregion

        #region View Event
        public virtual void ViewWillAppear()
        {

        }

        public virtual void ViewDidAppear()
        {

        }

        public virtual void ViewWillDisappear()
        {

        }

        public virtual void ViewDidDisappear()
        {

        }

        public virtual void OnViewEnable()
        {

        }

        public virtual void OnViewDisable()
        {

        }

        public virtual void OnViewAwake()
        {

        }

        public virtual void OnViewDestroy()
        {
            m_View = null;
        }

        #endregion

        #region Container
        protected List<IController> m_Children = new List<IController>();
        IController m_Parent;

        public List<IController> children
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
        public IController parent
        {
            get
            {
                return m_Parent;
            }
            set
            {
                m_Parent = value;
            }
        }

        public virtual void AddChildController(IController controller)
        {
            controller.WillMoveToParentController(this);
            m_Children.Add(controller);
            controller.parent = this;
            controller.DidMoveToParentController(this);
        }

        public virtual void RemoveFromParentController()
        {
            if (m_Parent != null)
            {
                WillMoveToParentController(null);
                m_Parent.children.Remove(this);
                m_Parent = null;
                DidMoveToParentController(null);
            }
        }

        public virtual void CleanChildren(bool recursion=false)
        {
            foreach (var c in m_Children)
            {
                c.WillMoveToParentController(null);
                c.parent = null;
                c.DidMoveToParentController(null);
                if (recursion)
                {
                    c.CleanChildren(recursion);
                }
            }
        }

        public virtual void WillMoveToParentController(IController parent)
        {

        }

        public virtual void DidMoveToParentController(IController parent)
        {

        }

        #endregion
    }
}
