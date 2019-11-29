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
            UIManager.Instance.ClosePanel(panel);
        }

        public void Confirm()
        {
            UIPanel panel = GetComponent<UIPanel>();
            UIManager.Instance.ClosePanel(panel);
        }
    }
}
