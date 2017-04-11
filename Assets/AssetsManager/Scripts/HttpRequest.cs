using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HttpRequest : MonoBehaviour {

    public delegate void  RequestCallback(string err, WWW www);

	public void Get(string url,RequestCallback callback)
    {
        StartCoroutine(Request(url, callback));
    }

    public void Post(string url, WWWForm form,RequestCallback callback)
    {
        StartCoroutine(Request(url, form, callback));
    }

    public void Post(string url, byte[] postData, RequestCallback callback)
    {
        StartCoroutine(Request(url, postData, callback));
    }

    public void Post(string url, byte[] postData, Dictionary<string, string> headers, RequestCallback callback)
    {
        StartCoroutine(Request(url, postData, headers,callback));
    }

    public IEnumerator Request(string url, RequestCallback callback)
    {
        WWW www = new WWW(url); ;

        yield return www;

        callback(www.error, www);
    }

    public IEnumerator Request(string url, WWWForm form,RequestCallback callback)
    {
        WWW  www = new WWW(url,form);

        yield return www;

        callback(www.error, www);
    }

    public IEnumerator Request(string url, byte[] postData,RequestCallback callback)
    {
        WWW www = new WWW(url, postData);

        yield return www;

        callback(www.error, www);
    }

    public IEnumerator Request(string url, byte[] postData, Dictionary<string, string> headers,RequestCallback callback)
    {
        WWW www = null;

        if (postData == null)
        {
            www = new WWW(url);
        }
        else
        {
            if(headers==null)
            {
                www = new WWW(url, postData);
            }
            else
            {
                www = new WWW(url, postData, headers);
            }           
        }

        yield return www;

        callback(www.error, www);
    }
}
