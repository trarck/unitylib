using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace YH.Update
{
    public class HttpClient : MonoBehaviour
    {
        int m_Timeout = 12;
        int m_RetryTimes = 3;

        public event System.Action<HttpClient> onComplete;
        public event System.Action<float> onProgress;

        UnityWebRequest m_Request;

        private void Update()
        {
            if (m_Request!=null && onProgress != null && !m_Request.isDone)
            {
                onProgress(m_Request.downloadProgress);
            }
        }

        private void OnDestroy()
        {
            if (m_Request != null)
            {
                m_Request.Dispose();
            }
        }


        public void Get(string url)
        {
            StartCoroutine(_Get(url));
        }

        protected IEnumerator _Get(string url)
        {
            m_Request = UnityWebRequest.Get(url);
            m_Request.timeout = m_Timeout;
#if SSH_ACCEPT_ALL
            m_Request.certificateHandler = new AcceptAllCertificatesSignedHandler();
#endif
            yield return m_Request.SendWebRequest();
            //Debug.LogFormat("NE:{0},HE:{1},Done:{2}", m_Request.isNetworkError, m_Request.isHttpError, m_Request.isDone);
            if (m_Request.isNetworkError && m_RetryTimes-->0)
            {
                Debug.Log("Network error retry times left:" + (m_RetryTimes + 1));
                //retry
                Get(url);
            }
            else if(onComplete!=null)
            {
                onComplete(this);
            }
        }

        public void Clear()
        {
            if (m_Request!=null)
            {
                m_Request.Dispose();
            }

            m_Request = null;
            onComplete = null;
            onProgress = null;
            m_RetryTimes = 3;
        }

        public int timeout
        {
            get
            {
                return m_Timeout;
            }
            set
            {
                m_Timeout = value;
            }
        }

        public int retryTimes
        {
            get
            {
                return m_RetryTimes;
            }
            set
            {
                m_RetryTimes = value;
            }
        }

        public UnityWebRequest request
        {
            get
            {
                return m_Request;
            }
        }
    }
}
