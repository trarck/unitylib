using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;
using YH.UI.Mvc;

namespace Test.UI.Mvc
{

    public class DialogController : MonoController
    {
        public void Cancel()
        {
            UIPanel panel = GetComponent<UIPanel>();
            panel.Close();
        }

        public void Confirm()
        {
            UIPanel panel = GetComponent<UIPanel>();
            panel.Close();
        }
    }
}
