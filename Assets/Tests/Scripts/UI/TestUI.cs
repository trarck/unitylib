using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH;
using YH.AssetManager;
using YH.UI;

public class TestUI : MonoBehaviour
{

    private void Awake()
    {
        AssetManager.Instance.Init();
        UIManager.Instance.Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.ShowDefault();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
