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
        ~SubAController()
        {
            Debug.Log("@@@Destroy SubAController");
        }

        public override void OnViewAwake()
        {
            Debug.Log("View Awake");
            base.OnViewAwake();
            //bind events
            BindEvents();
        }

        public override void OnViewDestroy()
        {
            Debug.Log("View destory");
            base.OnViewDestroy();
        }

        public override void OnViewEnable()
        {
            Debug.Log("View Enable");
            base.OnViewEnable();

        }

        public override void OnViewDisable()
        {
            Debug.Log("View Disable");
            base.OnViewDisable();
        }

        protected void BindEvents()
        {
            Dictionary<string, UnityEngine.Events.UnityAction> buttonClickEvents = new Dictionary<string, UnityEngine.Events.UnityAction>()
            {
                 {"GotoBtn",GotoB },
                 {"PushBtn",PushB },
                 {"PopBtn",Pop }
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
            TestMvc.rootController.Replace(subB);
            //subB.viewDidLoadHandle = (subBView) =>
            //{
            //    root.AddSubView(subBView);
            //    Dispose();
            //};
            //subB.LoadViewIfNeed();
        }

        public void PushB()
        {
            SubBController subB = new SubBController();
            subB.Init("Assets/Tests/Prefabs/UI/Mvc/SubBPanel.prefab");
            TestMvc.rootController.Push(subB);
        }

        public void Pop()
        {
            TestMvc.rootController.Pop();
        }
    }
}
