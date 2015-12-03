﻿using UnityEngine;
using System.Collections.Generic;

namespace YH
{
    public class SimpleSceneDirector : UnitySingleton<SimpleSceneDirector>, ISceneDirector
    {
        Dictionary<string, Scene> m_SceneMap;

        Stack<Scene> m_SceneStack = new Stack<Scene>();

        void Awake()
        {
            LoadScenesFromHierarchy();
        }

        public void LoadScenesFromHierarchy()
        {
            m_SceneMap = new Dictionary<string, Scene>();
            for (int i = 0; i < transform.childCount; ++i)
            {
                Scene scene = transform.GetChild(i).GetComponent<Scene>();
                if (scene != null)
                {
                    Debug.Log("add " + scene.gameObject.name);
                    m_SceneMap.Add(scene.gameObject.name, scene);
                }
            }
        }

        public void PopScene()
        {
            if (m_SceneStack.Count == 1)
            {
                Debug.Log("At root scene");
                return;
            }
            Scene topScene = m_SceneStack.Pop();
            Scene scene = m_SceneStack.Peek();
            topScene.Hide();
            scene.Show();
        }

        public void PopToRootScene()
        {
            PopToSceneStackLevel(1);
        }

        public void PopToSceneStackLevel(int level)
        {
            if (level <= 0)
            {
                Debug.LogError("PopToSceneStackLevel level must big zero");
            }

            int c = m_SceneStack.Count;
            while (c-- > level)
            {
                Scene scene = m_SceneStack.Pop();
                scene.Hide();
            }

            Scene current = m_SceneStack.Peek();
            current.Show();
        }

        public void PushScene(string sceneName)
        {
            Scene scene = m_SceneMap.ContainsKey(sceneName)? m_SceneMap[sceneName]: null;
            if (scene != null)
            {
                Scene topScene = m_SceneStack.Peek();

                m_SceneStack.Push(scene);

                topScene.Hide();

                scene.Show();
            }
        }

        public void ReplaceScene(string sceneName)
        {
            Scene scene = m_SceneMap.ContainsKey(sceneName) ? m_SceneMap[sceneName] : null;
            if (scene != null)
            {
                Scene topScene = m_SceneStack.Pop();

                m_SceneStack.Push(scene);
                topScene.Hide();
                scene.Show();
            }
        }

        public void RunWithScene(string sceneName)
        {
            Scene scene = m_SceneMap.ContainsKey(sceneName) ? m_SceneMap[sceneName] : null;
            if (scene != null)
            {
                m_SceneStack.Push(scene);
                scene.Show();
            }
        }
    }
}