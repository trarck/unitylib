using UnityEngine;
using System.Collections;
using YH;

public class TestHttp : MonoBehaviour {

    [SerializeField]
    HttpRequest m_HttpRequest;

	// Use this for initialization
	void Start () {
        string patchPacktUrl = "http://patch.xuebaogames.com/monster/1.0.0_1.0.6.zip";
        m_HttpRequest.Get(patchPacktUrl, (err, www) =>
        {
            if (err != null)
            {
                Debug.Log(err);
            }
            else
            {
                byte[] bytes = www.bytes;
                Debug.Log(bytes.Length);
            }
        });
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
