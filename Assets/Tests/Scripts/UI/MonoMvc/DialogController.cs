using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;
using YH.UI.MonoMvc;

namespace Test.UI.MonoMvc
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
