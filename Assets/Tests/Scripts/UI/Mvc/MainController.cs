using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;
using YH.UI.Mvc;

namespace Test.UI.Mvc
{

    public class MainController : Controller
    {
        public void GotoA()
        {
            UIManager.Instance.director.Replace("Assets/Tests/Prefabs/UI/Mvc/SubAPanel.prefab");
        }

        public void ShowDialog()
        {
            UIManager.Instance.ShowPanel("Assets/Tests/Prefabs/UI/Mvc/TestDialog1.prefab");
            UIManager.Instance.ShowPanel("Assets/Tests/Prefabs/UI/Mvc/TestDialog2.prefab");
        }
    }
}
