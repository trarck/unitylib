using UnityEngine;
using System.Collections;
using YH;

public class TestHttp : MonoBehaviour {

    [SerializeField]
    HttpRequest m_HttpRequest;

	// Use this for initialization
	void Start () {
        string patchPacktUrl = "http://patch.xuebaogames.com/monster/1.0.6.zip"; //"http://sw.bos.baidu.com/sw-search-sp/software/62fd1872480b4/iTunes_12.6.0.100_Setup.exe";// 
        m_HttpRequest.Get(patchPacktUrl, (err, www) =>
        {
            Debug.Log("End " + Time.frameCount);
            if (err != null)
            {
                Debug.Log(err);
            }
            else
            {
                byte[] bytes = www.bytes;
                Debug.Log(bytes.Length);
            }
        }, (percent) =>
        {
            Debug.LogFormat("state {0} {1}", percent, Time.frameCount);
        });
        Debug.Log("test start");

        //StartCoroutine(m_HttpRequest.Request(patchPacktUrl,
        //    (err, www)=>{
        //        Debug.Log("End "+err);
        //    },
        //    (p) =>
        //    {
        //        Debug.Log("p " + p);
        //    }
        //));
        //StartCoroutine(DownloadFromUrl(patchPacktUrl));
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator ShowProgress(WWW www)
    {
        Debug.Log("Show progress " + Time.frameCount);
        while (!www.isDone)
        {
            Debug.LogFormat("Download {0:p1} {1}", www.progress,Time.frameCount);
            yield return new WaitForEndOfFrame();//WaitForSeconds(0.1f);
        }
        Debug.Log("Done " + Time.frameCount);
    }

    private IEnumerator DownloadFromUrl(string strUrl)
    {
        Debug.Log("download start :"+Time.frameCount);
        WWW www = new WWW(strUrl);
        StartCoroutine(ShowProgress(www));
        yield return www;
        Debug.Log("test " + Time.frameCount);
    }
}
