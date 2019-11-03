using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YH.UI;
using YH.UI.Mvc;

namespace Test.UI.Mvc
{
    public class DialogController : Controller
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
                 {"CancelBtn",Cancel },
                 {"OkBtn",Confirm },
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

        public void Cancel()
        {
            Dispose();
        }

        public void Confirm()
        {
            Dispose();
        }
    }
}
