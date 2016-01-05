﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using YH;

namespace Simple
{
    public class StartScene : Scene
    {

        // Use this for initialization
        void Start()
        {
            RegisterButtonEvents(FindUtil.SearchComponents<Button>(transform), OnClick);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void RegisterButtonEvents(List<Button> buttons, UnityAction<GameObject> call)
        {
            foreach (Button btn in buttons)
            {
                GameObject obj = btn.gameObject;
                btn.onClick.AddListener(delegate ()
                {
                    call(obj);
                });
            }
        }

        void OnClick(GameObject sender)
        {
            switch (sender.name)
            {
                case "StartBtn":
                    SimpleSceneDirector.Instance.PushScene("GameScene");
                    break;

                case "PrevBtn":
                    SimpleSceneDirector.Instance.PopScene();
                    break;
            }
        }
    }
}