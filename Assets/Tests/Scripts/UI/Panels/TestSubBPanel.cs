using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;

public class TestSubBPanel : UIPanel
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GotoMain()
    {
        UIManager.Instance.director.Replace("Assets/Tests/Prefabs/UI/Panels/MainPanel.prefab");
    }
}
