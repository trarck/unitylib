using System;
using System.Collections.Generic;
using UnityEngine;

namespace YH.UI.Mvc
{
    public interface IController
    {
        IView view { get; set; }
        void Init();
        void Init(string viewAsset);
        void Dispose();
        //view
        void LoadView(Transform parent=null);
        void LoadViewIfNeed(Transform parent=null);
        void ViewDidLoad();
        void UnloadView();
        bool isViewLoaded { get; }

        void ViewWillAppear();
        void ViewDidAppear();
        void ViewWillDisappear();
        void ViewDidDisappear();
        //view event
        Action<IView> viewDidLoadHandle { get; set; }
        //unity event
        void OnViewEnable();
        void OnViewDisable();
        void OnViewAwake();
        void OnViewDestroy();
        //container
        List<IController> children { get; set; }
        IController parent { get; set; }
        void AddChildController(IController controller);
        void RemoveFromParentController();
        void CleanChildren(bool recursion);
        void WillMoveToParentController(IController parent);
        void DidMoveToParentController(IController parent);

    }
}
