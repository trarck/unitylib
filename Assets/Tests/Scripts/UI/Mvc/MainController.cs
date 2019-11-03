using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;
using YH.UI.Mvc;
using UnityEngine.UI;

namespace Test.UI.Mvc
{

    public class MainController : Controller
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
                 {"GotoABtn",GotoA },
                 {"ShowDaligBtn",ShowDialog },
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

        public void GotoA()
        {
            IView root = view.superView;

            SubAController subA = new SubAController();
            subA.Init("Assets/Tests/Prefabs/UI/Mvc/SubAPanel.prefab");
            subA.OnViewDidLoad += (subAView) =>
            {
                root.AddSubView(subAView);
                Dispose();
            };
            subA.LoadViewIfNeed();

        }

        public void ShowDialog()
        {
            IView root = view.superView;

            DialogController dialog = new DialogController();
            dialog.Init("Assets/Tests/Prefabs/UI/Mvc/TestDialog1.prefab");
            dialog.OnViewDidLoad += (dialogView) =>
            {
                root.AddSubView(dialogView);
            };
            dialog.LoadViewIfNeed();
        }
    }
}
