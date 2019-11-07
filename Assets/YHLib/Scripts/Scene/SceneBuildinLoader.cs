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
    public class SceneBuildinLoader:SceneLoader
    {

        public SceneBuildinLoader(SceneManager manager):base(manager)
        {

        }

        public override void Load(string scenePath, bool delayActive=false,ActiveHandle activeHandle=null)
        {
            m_ScenePath = scenePath;
            m_DelayActive = delayActive;
            m_ActiveHandle = activeHandle;

            //start load scene asset bundle
            SetState(State.LoadScene,"Start load scene");
            if (m_DelayActive)
            {
                m_Manager.StartCoroutine(_DelayLoad(scenePath));
            }
            else
            {
                m_Manager.StartCoroutine(_ActiveLoad(scenePath));
            }
        }
    }
}
