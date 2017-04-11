using UnityEngine;
using System.Collections;
using YH.AM;

public class TestAssetUpdate : MonoBehaviour {
    [SerializeField]
    AssetsUpdater m_AssetsUpdater;

    [SerializeField]
    string m_RemoteUrl;
	// Use this for initialization
	void Start ()
    {
        m_AssetsUpdater.StoragePath = Application.persistentDataPath;
        m_AssetsUpdater.UpdateUrl = m_RemoteUrl;
        m_AssetsUpdater.OnUpdating = (segment, err, msg, percent) =>{
            Debug.Log(msg+" p:"+percent);
        };
        m_AssetsUpdater.StartUpdate();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
