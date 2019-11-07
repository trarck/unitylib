using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YH.AssetManager;

namespace YH.Scene
{
    public class SceneManager:UnitySingleton<SceneManager>
    {
        public static string ScenePath = "Assets/Scenes";
        Dictionary<string, bool> m_BuildinScenes = new Dictionary<string, bool>();

        bool m_Inited = false;

        public void Init()
        {
            m_Inited = true;
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                var path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                //AddBuildinScene(path);
            }
        }

        public SceneLoader LoadScene(string scene,bool delayActive=false, SceneLoader.ActiveHandle activeHandle =null)
        {
            if (!m_Inited)
            {
                Init();
            }

            SceneLoader sceneLoader = null;
            string scenePath = GetScenePath(scene);

            if (IsBuildin(scenePath))
            {
                sceneLoader = new SceneBuildinLoader(this);
            }
            else
            {
                sceneLoader = new SceneLoader(this);
            }

//#if !UNITY_EDITOR || ASSET_BUNDLE_LOADER
//            sceneLoader = new SceneLoader(this);
//#else
//            sceneLoader = new SceneEditorLoader(this);
//#endif
            
            sceneLoader.Load(scenePath, delayActive, activeHandle);
            return sceneLoader;
        }

        public void AddBuildinScene(string scenePath)
        {
            scenePath = GetScenePath(scenePath);
            var shortName = Path.GetFileNameWithoutExtension(scenePath);
            var noAssetsName = scenePath.Replace("Assets/", "");
            m_BuildinScenes[scenePath] = true;
            m_BuildinScenes[shortName] = true;
            m_BuildinScenes[noAssetsName] = true;
        }

        public bool IsBuildin(string scenePath)
        {
            //直接查看
            if (m_BuildinScenes.ContainsKey(scenePath))
            {
                return true;
            }
            //没有扩展名
            string ext = Path.GetExtension(scenePath);
            string scenePathWithoutExt = scenePath.Replace(ext, "");
            if (m_BuildinScenes.ContainsKey(scenePathWithoutExt))
            {
                return true;
            }
            //使用名子
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            if (m_BuildinScenes.ContainsKey(sceneName))
            {
                return true;
            }
            //没有找到
            return false;
        }

        string GetScenePath(string scene)
        {
            if (!scene.StartsWith(ScenePath))
            {
                scene = ScenePath + "/" + scene;
            }

            if (Path.GetExtension(scene) != ".unity")
            {
                scene += ".unity";
            }

            return scene;
        }
    }
}
