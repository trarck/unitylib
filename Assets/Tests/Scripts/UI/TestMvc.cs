using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH;
using YH.AssetManager;
using YH.UI;
using Test.UI.Mvc;
using YH.UI.Mvc;

public class TestMvc : MonoBehaviour
{
    [SerializeField]
    View m_Root;

    private void Awake()
    {
        AssetManager.Instance.Init();
        UIManager.Instance.Init();

        if (m_Root == null)
        {
            m_Root = GetComponent < View> ();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MainController mainController = new MainController();
        mainController.Init("Assets/Tests/Prefabs/UI/Mvc/MainPanel.prefab");
        mainController.OnViewDidLoad += (view) =>
        {
            m_Root.AddSubView(view);
        };
        mainController.LoadViewIfNeed();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
