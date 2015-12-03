using UnityEngine;
using System.Collections.Generic;

namespace YH
{
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
                RectTransform rectTransform = sceneObj.GetComponent<RectTransform>();
                rectTransform.SetParent(transform);
                rectTransform.localScale = new Vector3(1, 1, 1);
                rectTransform.localPosition = Vector3.zero;

                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                return sceneObj.GetComponent<Scene>();
            }

            return null;
        }

        public void RunWithScene(Scene scene)
        {
            m_SceneStack.Push(scene);
            scene.Show();
        }

        public void PushScene(Scene scene)
        {
            Scene old = m_SceneStack.Peek();
            m_SceneStack.Push(scene);
            scene.Show();
            old.Hide();
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
            old.OnHide();
            Destroy(old.gameObject);

        }

        public void ReplaceScene(Scene scene)
        {
            Scene old = m_SceneStack.Pop();
            m_SceneStack.Push(scene);
            scene.Show();
            old.OnHide();
            Destroy(old.gameObject);
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
                scene.OnHide();
                Destroy(scene);
            }

            Scene current = m_SceneStack.Peek();
            current.Show();
        }

        public void PopToRootScene()
        {
            PopToSceneStackLevel(1);
        }

        public void RunWithScene(string sceneName)
        {
            Scene scene = LoadSceneFromAssets(sceneName);
            RunWithScene(scene);
        }

        public void PushScene(string sceneName)
        {
            Debug.Log("push:" + sceneName);
            Scene scene = LoadSceneFromAssets(sceneName);
            PushScene(scene);
        }

        public void ReplaceScene(string sceneName)
        {
            Scene scene = LoadSceneFromAssets(sceneName);
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