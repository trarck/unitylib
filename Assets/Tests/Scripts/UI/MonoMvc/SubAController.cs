using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;
using YH.UI.MonoMvc;

namespace Test.UI.MonoMvc
{

    public class SubAController : MonoController
    {
        public void GotoB()
        {
            UIManager.Instance.director.Replace("Assets/Tests/Prefabs/UI/MonoMvc/SubBPanel.prefab");
        }
    }
}
