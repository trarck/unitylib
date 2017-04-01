using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

namespace YH.AM
{
    public class AssetsUpdater : MonoBehaviour
    {

        public enum UpdateSegment
        {
            CompareVersion,
            DownloadAssets,
            ApplyAssets,
            Complete
        }

        public enum UpdateError
        {
            OK,
            //下载远程版本信息错误
            DownloadRemoteVersionError,
            //解析远程版本信息错误
            ParseRemoteVersionError,
            //主体版本太旧不被支持
            HostIsOutOfDate,
            //当前版本太旧不被支持
            CurrentVersionOutOfDate,
        }

        public static string CurrentVersionName = "CurrentVersion";

        public static string HostVersionName = "HostVersion";

        //更新地址
        string m_UpdateUrl;

        //本地存储位置
        string m_StoragePath;

        //本地版本
        Version m_CurrentVersion;

        //主体版本
        Version m_HostVersion;

        //主体版本是否在文件中。如果为ture,则由程序启动时写到文件中。
        bool m_HostVersionInFile=true;

        public delegate void UpdatingHandle(UpdateSegment segment, UpdateError err, string msg, float percent);
        public UpdatingHandle OnUpdating;

        //开始更新
        public int StartUpdate()
        {
            StartCoroutine(GetRemoteVersion());
            return 0;
        }

        protected void CompareVersion(RemoteVersions remoteVersions)
        {
            //get local version
            m_CurrentVersion = GetCurrentVersion();
            m_HostVersion = GetHostVersion();

            //first check the host is support
            if(remoteVersions.MinHostVersion>m_HostVersion)
            {
                //the host is out data
                TriggerUpdating(UpdateSegment.CompareVersion,UpdateError.HostIsOutOfDate, "please download the newest application", 1);
                return;
            }

            //check min support version
            if(remoteVersions.MinSupportVersion> m_CurrentVersion)
            {
                TriggerUpdating(UpdateSegment.CompareVersion,UpdateError.CurrentVersionOutOfDate, "please download the newest application", 1);
                return;
            }

            //check is latest version
            if(m_CurrentVersion>=remoteVersions.LatestVersion)
            {
                //the current is latest version,complete the update
                TriggerUpdating(UpdateSegment.Complete, UpdateError.CurrentVersionOutOfDate, "please download the newest application", 1);
                return;
            }

            //do update
            string patchPath = GetPatchPath(m_CurrentVersion, remoteVersions.LatestVersion);
            //TODO download the manifest file.

            //download the pack file
        }

        public IEnumerator GetRemoteVersion()
        {
            string remoteVersionUrl = m_UpdateUrl + "/version.txt";

            WWW www = new WWW(remoteVersionUrl);
            yield return www;

            if (www.error != null)
            {
                //获取远程版本信息失败
                OnUpdating(UpdateSegment.CompareVersion, UpdateError.DownloadRemoteVersionError, "download fail:" + remoteVersionUrl, 1);
            }
            else
            {
                RemoteVersions remoteVersions = new RemoteVersions();
                if(remoteVersions.Parse(www.text))
                {
                    CompareVersion(remoteVersions);
                }
                else
                {
                    OnUpdating(UpdateSegment.CompareVersion, UpdateError.ParseRemoteVersionError, "parse fail:" + www.text, 1);
                }
            }
        }

        public string GetPatchPath(Version from,Version to)
        {
            return from.ToString() + "_" + to.ToString();
        }

        public Version GetCurrentVersion()
        {
            Version ver = null;
            string content = File.ReadAllText(Path.Combine(m_StoragePath, CurrentVersionName));
            if(!string.IsNullOrEmpty(content) && Version.IsVersionFormat(content.Trim()))
            {
                ver = new Version(content);
            }            
            return ver;
        }

        public Version GetHostVersion()
        {
            Version ver = null;
            if (Version.IsVersionFormat(Application.version))
            {
                ver = new Version(Application.version);
            }
            return ver;
        }

        protected void TriggerUpdating(UpdateSegment segment, UpdateError err, string msg, float percent)
        {
            if (OnUpdating != null)
                OnUpdating(segment,err, msg, percent);
        }

        public string UpdateUrl
        {
            set
            {
                m_UpdateUrl = value;
            }

            get
            {
                return m_UpdateUrl;
            }
        }

        public bool IsHostVersionInFile
        {
            set
            {
                m_HostVersionInFile = value;
            }

            get
            {
                return m_HostVersionInFile;
            }
        }
    }
}