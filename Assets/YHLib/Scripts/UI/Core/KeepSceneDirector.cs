using UnityEngine;
using System.Collections.Generic;

namespace YH.UI
{
    /// <summary>
    /// 把Scene的实体放入栈，会从外部创建Scene。
    /// 如果栈中有多个同名的Scene,同名的Scene只有一个实体,这样保持Scene的一致性。
    /// </summary>
    public class KeepSceneDirector : UnitySingleton<KeepSceneDirector>, ISceneDirector
    {
        public static string sceneDir = "Scenes/";

        Dictionary<string, GameObject> m_ScenePrefabMap = new Dictionary<string, GameObject>();

        Stack<Scene> m_SceneStack = new Stack<Scene>();

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
                //sceneObj.name = name;

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

        public void RunWithScene(Scene scene)
        {
            if (scene != null)
            {
                m_SceneStack.Push(scene);
                scene.Show();
            }
        }

        public void PushScene(Scene scene)
        {
            if (scene != null)
            {
                Scene old = m_SceneStack.Peek();
                m_SceneStack.Push(scene);
                scene.Show();
                old.Hide();
            }
        }

        public void PopScene()
        {
            if (m_SceneStack.Count == 1)
            {
                Debug.Log("At root scene");
                return;
            }

            Scene old = m_SceneStack.Pop();
            Scene current = m_SceneStack.Peek();
            current.Show();
            old.Hide();

            if (!IsUsing(old))
            {
                Destroy(old.gameObject);
            }
        }

        public void ReplaceScene(Scene scene)
        {
            if (scene != null)
            {
                Scene old = m_SceneStack.Pop();
                m_SceneStack.Push(scene);
                scene.Show();
                old.Hide();

                if (!IsUsing(old))
                {
                    Destroy(old.gameObject);
                }                
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
                Scene scene = m_SceneStack.Pop();
                scene.Hide();
                if (!IsUsing(scene))
                {
                    Destroy(scene);
                }
            }

            Scene current = m_SceneStack.Peek();
            current.Show();
        }

        public void PopToRootScene()
        {
            PopToSceneStackLevel(1);
        }

        bool IsUsing(Scene scene)
        {
            foreach (Scene iter in m_SceneStack)
            {
                if (iter == scene)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获得Scene
        /// 从当前栈中找，找到返回，否则创建一个新的。
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        Scene GetScene(string sceneName)
        {
            //look in stack
            foreach (Scene scene in m_SceneStack)
            {
                if (scene.name == sceneName)
                {
                    return scene;
                }
            }

            return LoadSceneFromAssets(sceneName);
        }

        public void RunWithScene(string sceneName)
        {
            Scene scene = GetScene(sceneName);
            RunWithScene(scene);
        }

        public void PushScene(string sceneName)
        {
            Scene scene = GetScene(sceneName);
            PushScene(scene);
        }

        public void ReplaceScene(string sceneName)
        {
            Scene scene = GetScene(sceneName);
            ReplaceScene(scene);
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