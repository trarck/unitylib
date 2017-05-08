using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace YH.Net
{
    public class HttpRequest : MonoBehaviour
    {

        public delegate void RequestCallback(string err, WWW www);
        public delegate void ProgressCallback(float percent);

        public float progressInterval = 0.1f;

        public void Get(string url, RequestCallback callback, ProgressCallback progressCallback = null)
        {
            StartCoroutine(Request(url, callback, progressCallback));
        }

        public void Post(string url, WWWForm form, RequestCallback callback, ProgressCallback progressCallback = null)
        {
            StartCoroutine(Request(url, form, callback, progressCallback));
        }

        public void Post(string url, byte[] postData, RequestCallback callback, ProgressCallback progressCallback = null)
        {
            StartCoroutine(Request(url, postData, callback, progressCallback));
        }

        public void Post(string url, byte[] postData, Dictionary<string, string> headers, RequestCallback callback, ProgressCallback progressCallback = null)
        {
            StartCoroutine(Request(url, postData, headers, callback, progressCallback));
        }

        public IEnumerator Request(string url, RequestCallback callback)
        {
            WWW www = new WWW(url);

            yield return www;

            callback(www.error, www);
        }

        public IEnumerator Request(string url, RequestCallback callback, ProgressCallback progressCallback)
        {
            WWW www = new WWW(url);
            if (progressCallback != null)
            {
                StartCoroutine(HandleProgress(www, progressCallback));
            }
            yield return www;
            callback(www.error, www);
        }

        public IEnumerator Request(string url, WWWForm form, RequestCallback callback)
        {
            WWW www = new WWW(url, form);

            yield return www;

            callback(www.error, www);
        }

        public IEnumerator Request(string url, WWWForm form, RequestCallback callback, ProgressCallback progressCallback)
        {
            WWW www = new WWW(url, form);
            if (progressCallback != null)
            {
                StartCoroutine(HandleProgress(www, progressCallback));
            }
            yield return www;
            callback(www.error, www);
        }

        public IEnumerator Request(string url, byte[] postData, RequestCallback callback)
        {
            WWW www = new WWW(url, postData);

            yield return www;

            callback(www.error, www);
        }

        public IEnumerator Request(string url, byte[] postData, RequestCallback callback, ProgressCallback progressCallback)
        {
            WWW www = new WWW(url, postData);
            if (progressCallback != null)
            {
                StartCoroutine(HandleProgress(www, progressCallback));
            }
            yield return www;
            callback(www.error, www);
        }

        public IEnumerator Request(string url, byte[] postData, Dictionary<string, string> headers, RequestCallback callback, ProgressCallback progressCallback = null)
        {
            WWW www = null;

            if (postData == null)
            {
                www = new WWW(url);
            }
            else
            {
                if (headers == null)
                {
                    www = new WWW(url, postData);
                }
                else
                {
                    www = new WWW(url, postData, headers);
                }
            }

            if (progressCallback != null)
            {
                StartCoroutine(HandleProgress(www, progressCallback));
            }

            yield return www;

            callback(www.error, www);
        }
        private IEnumerator HandleProgress(WWW www, ProgressCallback progressCallback)
        {
            while (!www.isDone)
            {
                progressCallback(www.progress);
                yield return new WaitForSeconds(progressInterval); // WaitForEndOfFrame();
            }
            //这里不需要再调用百分白时候的回调,在这以前已经执行RequestCallback的回调.可以在RequestCallback里把显示进度设置成100%
            //progressCallback(1.0f);
        }
    }
}