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
        ~MainController()
        {
            Debug.Log("@@@Destroy MainController");
        }

        public override void OnViewAwake()
        {
            Debug.Log("Main View Awake");
            base.OnViewAwake();
            //bind events
            BindEvents();
        }

        public override void OnViewDestroy()
        {
            Debug.Log("Main View destory");
            base.OnViewDestroy();
        }

        public override void OnViewEnable()
        {
            Debug.Log("Main View Enable");
            base.OnViewEnable();

        }

        public override void OnViewDisable()
        {
            Debug.Log("Main View Disable");
            base.OnViewDisable();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Debug.Log("Main ViewDidLoad");


        }

        protected void BindEvents()
        {
            Dictionary<string, UnityEngine.Events.UnityAction> buttonClickEvents = new Dictionary<string, UnityEngine.Events.UnityAction>()
            {
                 {"GotoABtn",GotoA },
                 {"ShowDaligBtn",ShowDialog },
                 {"PopBtn",Pop },
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
            Debug.Log("### GotoA");
            IView root = view.superView;

            SubAController subA = new SubAController();
            subA.Init("Assets/Tests/Prefabs/UI/Mvc/SubAPanel.prefab");

            TestMvc.rootController.Replace(subA);
            //subA.viewDidLoadHandle= (subAView) =>
            //{
            //    root.AddSubView(subAView);
            //    Dispose();
            //};
            //subA.LoadViewIfNeed();
        }

        public void Pop()
        {
            TestMvc.rootController.Pop();
        }

        public void ShowDialog()
        {
            IView root = view.superView;

            DialogController dialog = new DialogController();
            dialog.Init("Assets/Tests/Prefabs/UI/Mvc/TestDialog1.prefab");
            dialog.viewDidLoadHandle = (dialogView) =>
            {
                root.AddSubView(dialogView);
            };
            dialog.LoadViewIfNeed();
        }
    }
}
