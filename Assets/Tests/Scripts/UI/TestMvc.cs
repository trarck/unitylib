using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH;
using YH.AssetManager;
using YH.UI;
using Test.UI.Mvc;

public class TestMvc : MonoBehaviour
{
    private void Awake()
    {
        AssetManager.Instance.Init();
        UIManager.Instance.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        MainController mainController = new MainController();
        mainController.Init("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
