using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YH.UI;
using YH.UI.Mvc;

namespace Test.UI.Mvc
{

    public class SubAController : Controller
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
                 {"Button",GotoB }
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


        public void GotoB()
        {
            IView root = view.superView;

            SubBController subB= new SubBController();
            subB.Init("Assets/Tests/Prefabs/UI/Mvc/SubBPanel.prefab");
            subB.viewDidLoadHandle = (subBView) =>
            {
                root.AddSubView(subBView);
                Dispose();
            };
            subB.LoadViewIfNeed();
        }
    }
}
