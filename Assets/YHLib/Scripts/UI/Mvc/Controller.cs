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

        public virtual void Init()
        {

        }

        public virtual void Init(string viewAsset)
        {
            m_ViewAsset=viewAsset;
        }

        public virtual void LoadView()
        {
            //Load from asset file

            //Create empty
            CreateEmpyView();
        }

        public virtual void ViewDidLoad()
        {

        }

        void CreateEmpyView()
        {
            GameObject viewObj = new GameObject();
            m_View = viewObj.AddComponent<View>();
        }
    }
}
