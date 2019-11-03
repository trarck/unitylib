using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YH.UI.Mvc
{
    public interface IController
    {
        IView view { get; set; }
        void Init();
        void Init(string viewAsset);
        void Dispose();
        //view
        void LoadView();
        void LoadViewIfNeed();
        void ViewDidLoad();
        void UnloadView();
        bool isViewLoaded { get; }

        void ViewWillAppear();
        void ViewDidAppear();
        void ViewWillDisappear();
        void ViewDidDisappear();
        //view event
        event Action<IView> OnViewDidLoad;
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
