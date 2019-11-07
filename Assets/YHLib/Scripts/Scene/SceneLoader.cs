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
    public class SceneLoader
    {
        public enum State
        {
            Idle,
            LoadAssets,
            LoadScene,
            WaitForActive,
            Complete,
            Error
        }
        
        public delegate void ProcessHandle(State segment, string msg);
        public event ProcessHandle onProcess;

        protected State m_State;

        protected string m_ScenePath = null;
        protected bool m_DelayActive = false;

        public delegate bool ActiveHandle();
        protected ActiveHandle m_ActiveHandle = null;

        protected SceneManager m_Manager;

        public SceneLoader(SceneManager manager)
        {
            m_Manager = manager;
        }

        public virtual void Load(string scenePath, bool delayActive=false,ActiveHandle activeHandle=null)
        {
            m_ScenePath = scenePath;
            m_DelayActive = delayActive;
            m_ActiveHandle = activeHandle;

            //start load scene asset bundle
            SetState(State.LoadAssets,"Start load assets");
            LoadSceneAssets(scenePath);
        }

        protected void LoadSceneAssets(string scenePath)
        {
            AssetManager.AssetManager.Instance.LoadScene(scenePath, 0, OnAssetsLoaded);
        }

        protected void OnAssetsLoaded(AssetBundleReference abr)
        {
            if (abr != null)
            {
                string[] scenes = abr.assetBundle.GetAllScenePaths();
                if(scenes!=null && scenes.Length > 0)
                {
                    if (m_DelayActive)
                    {
                        m_Manager.StartCoroutine(_DelayLoad(scenes[0]));
                    }
                    else
                    {
                        m_Manager.StartCoroutine(_ActiveLoad(scenes[0]));
                    }
                }
                else
                {
                    SetState(State.Error, "AssetBundle:"+ abr.name + " not have scene");
                }
            }
            else
            {
                SetState(State.Error, "Load assets fail");
            }
        }

        protected IEnumerator _ActiveLoad(string scene)
        {
            SetState(State.LoadScene, "Start Load scene");
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
            DoLoadComplete();
        }

        protected IEnumerator _DelayLoad(string scene)
        {
            SetState(State.LoadScene, "Start Load scene");
            AsyncOperation asyncOperation =UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                // Check if the load has finished
                if (asyncOperation.progress >= 0.9f && asyncOperation.allowSceneActivation==false)
                {
                    SetState(State.WaitForActive, "Wait scene active");
                    asyncOperation.allowSceneActivation = m_ActiveHandle();
                }
                yield return null;
            }
            DoLoadComplete();
        }

        protected void DoLoadComplete()
        {
            SetState(State.Complete, "Load scene complete");
        }

        protected void SetState(State state,string msg)
        {
            m_State = state;
            Debug.Log(msg);
            if (onProcess != null)
            {
                onProcess.Invoke(state, msg);
            }
        }

        public void Clean()
        {
            m_State = State.Idle;
            m_ScenePath = null;
            m_DelayActive = false;
            m_ActiveHandle = null;
            onProcess = null;
        }

        public void Dispose()
        {
            Clean();
        }

        public State state
        {
            get
            {
                return m_State;
            }
        }
    }
}
