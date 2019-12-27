using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace YH.Update
{
    public class HttpClient : MonoBehaviour
    {
        protected enum State
        {
            None,
            Requesting,
            Requested
        }

        protected State m_State = State.None;

        int m_Timeout = 12;
        int m_RetryTimes = 3;

        public event System.Action<HttpClient> onComplete;
        public event System.Action<float> onProgress;

        UnityWebRequest m_Request;
        bool m_haveRequest = false;
        string m_Url;

        private void Update()
        {
            if (m_State==State.Requesting && m_Request != null)
            {
                if (m_Request.isDone)
                {
                    if (m_Request.isNetworkError && m_RetryTimes-- > 0)
                    {
                        YH.Log.YHDebug.LogFormat("Network error retry times left:{0}", (m_RetryTimes + 1));
                        //retry
                        _Get1(m_Url);
                    }
                    else
                    {
                        m_State = State.Requested;
                        if (onComplete != null)
                        {
                            onComplete(this);
                        }
                    }
                }
                else if (onProgress != null)
                {
                    onProgress(m_Request.downloadProgress);
                }
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
            m_Url = url;
            //StartCoroutine(_Get(url));
            _Get1(url);
        }

        protected void _Get1(string url)
        {
            m_State = State.Requesting;

            if (m_Request != null)
            {
                m_Request.Dispose();
            }

            m_Request = UnityWebRequest.Get(url);
            m_Request.timeout = m_Timeout;
#if SSH_ACCEPT_ALL
            m_Request.certificateHandler = new AcceptAllCertificatesSignedHandler();
#endif
            m_Request.SendWebRequest();
        }

        protected IEnumerator _Get(string url)
        {
            if (m_Request != null)
            {
                m_Request.Dispose();
            }

            m_Request = UnityWebRequest.Get(url);
            m_Request.timeout = m_Timeout;
#if SSH_ACCEPT_ALL
            m_Request.certificateHandler = new AcceptAllCertificatesSignedHandler();
#endif
            yield return m_Request.SendWebRequest();
            //Debug.LogFormat("NE:{0},HE:{1},Done:{2}", m_Request.isNetworkError, m_Request.isHttpError, m_Request.isDone);
            if (m_Request.isNetworkError && m_RetryTimes-->0)
            {
                YH.Log.YHDebug.LogFormat("Network error retry times left:{0}" ,(m_RetryTimes + 1));
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
