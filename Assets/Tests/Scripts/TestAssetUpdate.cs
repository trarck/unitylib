using UnityEngine;
using System.Collections;
using YH.AM;
using UnityEngine.UI;

public class TestAssetUpdate : MonoBehaviour {
    [SerializeField]
    AssetsUpdater m_AssetsUpdater;

    [SerializeField]
    string m_RemoteUrl;

    [SerializeField]
    Slider m_ProgressBar;

    [SerializeField]
    Text m_ProgressMsg;

	// Use this for initialization
	void Start ()
    {
        m_AssetsUpdater.StoragePath = Application.persistentDataPath;
        m_AssetsUpdater.UpdateUrl = m_RemoteUrl;
        m_AssetsUpdater.OnUpdating += (segment, err, msg, percent) =>{
            Debug.LogFormat("updating seg:{3}, err:{0},msg:{1},percent:{2},frame:{4}",err,msg,percent,segment,Time.frameCount);
            m_ProgressMsg.text = msg;
            m_ProgressBar.value = percent;
            if (err != AssetsUpdater.UpdateError.OK)
            {
                //show dialog
            }
            if (AssetsUpdater.UpdateSegment.Complete == segment)
            {
                Debug.Log("###更新完成###");
            }
        };
        m_AssetsUpdater.StartUpdate();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
