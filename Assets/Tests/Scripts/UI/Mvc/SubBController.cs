using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;
using YH.UI.Mvc;

namespace Test.UI.Mvc
{

    public class SubBController : MonoController
    {
        public void GotoMain()
        {
            UIManager.Instance.director.Replace("Assets/Tests/Prefabs/UI/Mvc/MainPanel.prefab");
        }
    }
}
