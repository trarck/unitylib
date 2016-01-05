using UnityEngine;
using System.Collections.Generic;

namespace YH
{
    /// <summary>
    /// Scene的名字放入栈。
    /// 用到Scene时会重新创建，没办法记住Scene的状态。通常Scene没有状态，都是创建的时候加载数据显示。
    /// </summary>
    public class SceneDirector : UnitySingleton<SceneDirector>, ISceneDirector
    {
        public static string sceneDir = "Scenes/";

        Dictionary<string, GameObject> m_ScenePrefabMap=new Dictionary<string, GameObject>();

        Stack<string> m_SceneStack = new Stack<string>();

        Scene m_RunningScene = null;

        public GameObject LoadScenePrefabFromAssets(string name)
        {
            return Resources.Load<GameObject>(sceneDir + name);
        }

        public GameObject GetScenePrefab(string name)
        {
            if (m_ScenePrefabMap.ContainsKey(name))
            {
                return m_ScenePrefabMap[name];
            }
            else
            {
                GameObject prefab = LoadScenePrefabFromAssets(name);
                m_ScenePrefabMap[name] = prefab;
                return prefab;
            }
        }

        public Scene LoadSceneFromAssets(string name)
        {
            GameObject scenePrefab = GetScenePrefab(name);

            if (scenePrefab != null)
            {
                GameObject sceneObj = Instantiate(scenePrefab);
                RectTransform rectTransform = sceneObj.GetComponent<RectTransform>();
                rectTransform.SetParent(transform);
                rectTransform.localScale = new Vector3(1, 1, 1);
                rectTransform.localPosition = Vector3.zero;

                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                Scene scene = sceneObj.GetComponent<Scene>();
                scene.name = name;
                return scene;
            }

            return null;
        }

        public void RunWithScene(string sceneName)
        {
            Scene scene = LoadSceneFromAssets(sceneName);
            if (scene != null)
            {
                m_SceneStack.Push(sceneName);
                scene.Show();

                m_RunningScene = scene;
            }
        }

        public void PushScene(string sceneName)
        {
            if (m_RunningScene == null)
            {
                RunWithScene(sceneName);
            }
            else
            {
                Scene scene = LoadSceneFromAssets(sceneName);
                if (scene != null)
                {
                    m_SceneStack.Push(sceneName);

                    scene.Show();

                    m_RunningScene.OnHide();
                    Destroy(m_RunningScene.gameObject);

                    m_RunningScene = scene;
                }
            }
        }

        public void PopScene()
        {
            if(m_SceneStack.Count==1)
            {
                Debug.Log("At root scene");
                return;
            }
            m_SceneStack.Pop();

            string sceneName = m_SceneStack.Peek();
            Scene scene = LoadSceneFromAssets(sceneName);
            if (scene != null)
            {
                scene.Show();

                m_RunningScene.Hide();
                Destroy(m_RunningScene.gameObject);

                m_RunningScene = scene;
            }
        }

        public void ReplaceScene(string sceneName)
        {
            m_SceneStack.Pop();

            m_SceneStack.Push(sceneName);

            Scene scene = LoadSceneFromAssets(sceneName);
            if (scene != null)
            {
                scene.Show();

                m_RunningScene.Hide();
                Destroy(m_RunningScene.gameObject);

                m_RunningScene = scene;
            }
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
                m_SceneStack.Pop();
            }

            string sceneName = m_SceneStack.Peek();
            Scene scene = LoadSceneFromAssets(sceneName);
            if (scene != null)
            {
                scene.Show();

                m_RunningScene.Hide();
                Destroy(m_RunningScene.gameObject);

                m_RunningScene = scene;
            }
        }

        public void PopToRootScene()
        {
            PopToSceneStackLevel(1);
        }

        public Dictionary<string, GameObject> scenePrefabMap
        {
            set
            {
                m_ScenePrefabMap = value;
            }

            get
            {
                return m_ScenePrefabMap;
            }
        }
    }
}