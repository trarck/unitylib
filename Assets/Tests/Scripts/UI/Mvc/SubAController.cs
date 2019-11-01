using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;
using YH.UI.Mvc;

namespace Test.UI.Mvc
{

    public class SubAController : Controller
    {
        public void GotoB()
        {
            UIManager.Instance.director.Replace("Assets/Tests/Prefabs/UI/Mvc/SubBPanel.prefab");
        }
    }
}
