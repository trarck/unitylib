using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;

public class TestMainPanel : UIPanel
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GotoA()
    {
        UIManager.Instance.director.Replace("Assets/Tests/Prefabs/UI/Panels/SubAPanel.prefab");
    }

    public void ShowDialog()
    {
        UIManager.Instance.ShowPanel("Assets/Tests/Prefabs/UI/Dialogs/TestDialog1.prefab");
        UIManager.Instance.ShowPanel("Assets/Tests/Prefabs/UI/Dialogs/TestDialog2.prefab");
    }
}
