using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YH.UI;
using YH.UI.Mvc;

namespace Test.UI.Mvc
{
    public class SubBController : Controller
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //bind events
            BindEvents();
        }

        protected void BindEvents()
        {
            Dictionary<string, UnityEngine.Events.UnityAction> buttonClickEvents = new Dictionary<string, UnityEngine.Events.UnityAction>()
            {
                 {"Button",GotoMain }
            };

            foreach (var iter in buttonClickEvents)
            {
                GameObject btnObj = YH.HierarchyUtil.FindGameObject(iter.Key, view.rectTransorm);
                if (btnObj)
                {
                    Button button = btnObj.GetComponent<Button>();
                    button.onClick.AddListener(iter.Value);
                }
            }
        }

        public void GotoMain()
        {
            IView root = view.superView;

            MainController mainController = new MainController();
            mainController.Init("Assets/Tests/Prefabs/UI/Mvc/MainPanel.prefab");
            mainController.viewDidLoadHandle= (view) =>
            {
                root.AddSubView(view);
                Dispose();
            };
            mainController.LoadViewIfNeed();
        }
    }
}
