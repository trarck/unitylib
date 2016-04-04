using UnityEngine;
using System.Collections.Generic;

namespace YH.UI
{
    /// <summary>
    /// Scene已经放在游戏里，不需创建。
    /// 栈中同名的Scene，都是同一个。可以保存状态。
    /// </summary>
    public class SimpleSceneDirector : UnitySingleton<SimpleSceneDirector>, ISceneDirector
    {
        Dictionary<string, Scene> m_SceneMap;

        Stack<Scene> m_SceneStack = new Stack<Scene>();

        [SerializeField]
        Transform[] m_Containers;

        void Awake()
        {
            LoadScenes();
        }

        protected virtual void LoadScenes()
        {
            m_SceneMap = new Dictionary<string, Scene>();
            //load scene from containers
            if (m_Containers.Length>0)
            {
                for(int i = 0; i < m_Containers.Length; ++i)
                {
                    LoadScenesFromHierarchy(m_Containers[i]);
                }
            }
            //load scene from self
            LoadScenesFromHierarchy(this.transform);
        }

        public void LoadScenesFromHierarchy(Transform container)
        {
            for (int i = 0; i < container.childCount; ++i)
            {
                Scene scene = container.GetChild(i).GetComponent<Scene>();
                if (scene != null)
                {
                    Debug.Log("SimpleSceneDirector Add " + scene.gameObject.name);
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