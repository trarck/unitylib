using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using Ionic.Zip;
using YH.Net;

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
            //下载manifest文件信错
            DownloadManifestError,
            //下载补丁包文件信错
            DownloadPatchPackError,
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

        [SerializeField]
        HttpRequest m_HttpRequest;

        /// <summary>
        /// 开始更新
        /// 因为不是同步，执行流程分散在各个子函数里。
        /// 由于下一步依赖上一步的数据，无法直接使用Coroutine来写成同步调用。可以使用成员变量来保存。
        /// </summary>
        /// <returns></returns>
        public int StartUpdate()
        {
            GetRemoteVersion(); //Step
            return 0;
        }
        
        public void GetRemoteVersion()
        {
            string remoteVersionUrl = m_UpdateUrl + "/version.txt";
            OnUpdating(UpdateSegment.CompareVersion, UpdateError.OK, "Get Remote Version", 0);
            m_HttpRequest.Get(remoteVersionUrl, (err, www) =>
            {
                if (err != null)
                {
                    //获取远程版本信息失败
                    OnUpdating(UpdateSegment.CompareVersion, UpdateError.DownloadRemoteVersionError, "download fail:" + remoteVersionUrl, 1);
                }
                else
                {
                    //获取远程版本信息成功
                    OnUpdating(UpdateSegment.CompareVersion, UpdateError.OK, "Get Remote Version", 0.2f);
                    RemoteVersions remoteVersions = new RemoteVersions();
                    if (remoteVersions.Parse(www.text))
                    {
                        //比较版本
                        CompareVersion(remoteVersions); //Step
                    }
                    else
                    {
                        OnUpdating(UpdateSegment.CompareVersion, UpdateError.ParseRemoteVersionError, "parse fail:" + www.text, 1);
                    }
                }
            });
        }

        protected void CompareVersion(RemoteVersions remoteVersions)
        {
            //get local version
            m_CurrentVersion = GetCurrentVersion();
            m_HostVersion = GetHostVersion();

            if(m_CurrentVersion==null)
            {
                m_CurrentVersion = m_HostVersion;
            }

            //first check the host is support
            if (remoteVersions.MinHostVersion > m_HostVersion)
            {
                //the host is out data
                TriggerUpdating(UpdateSegment.CompareVersion, UpdateError.HostIsOutOfDate, "please download the newest application", 1);
                return;
            }

            //check min support version
            if (remoteVersions.MinSupportVersion > m_CurrentVersion)
            {
                TriggerUpdating(UpdateSegment.CompareVersion, UpdateError.CurrentVersionOutOfDate, "please download the newest application", 1);
                return;
            }

            //check is latest version
            if (m_CurrentVersion >= remoteVersions.LatestVersion)
            {
                //the current is latest version,complete the update
                TriggerUpdating(UpdateSegment.Complete, UpdateError.CurrentVersionOutOfDate, "the assets is latest did not need update", 1);
                return;
            }

            OnUpdating(UpdateSegment.CompareVersion, UpdateError.OK, "Compare Complete", 1);
            //do update
            string patchPath = GetPatchPath(m_CurrentVersion, remoteVersions.LatestVersion);
            //TODO download the manifest file.
            //GetManifestHeaderFile(patchPath);
            //download the pack file            
            DownLoadPatchPack(patchPath);//Step

            //更新当前版本号，但不保存到文件.等所有补丁执行完成才保存到本地文件。
            m_CurrentVersion = remoteVersions.LatestVersion;
        }

        public void GetManifestHeaderFile(string patchPath)
        {
            string manifestUrl = m_UpdateUrl +"/"+ patchPath+".manifest";

            m_HttpRequest.Get(manifestUrl, (err, www) =>
            {
                if (err != null)
                {
                    //获取manifest信息失败
                    OnUpdating(UpdateSegment.DownloadAssets, UpdateError.DownloadManifestError, "download fail:" + manifestUrl, 1);
                }
                else
                {
                    if (string.IsNullOrEmpty(www.text))
                    {
                        OnUpdating(UpdateSegment.DownloadAssets, UpdateError.DownloadManifestError, "Manifest file is empty:" + manifestUrl, 1);
                    }
                    else
                    {
                        ManifestHeader manifestHeader = JsonUtility.FromJson<ManifestHeader>(www.text);
                        if(manifestHeader==null)
                        {
                            OnUpdating(UpdateSegment.DownloadAssets, UpdateError.DownloadManifestError, "parse fail:" + www.text, 1);
                        }
                        else
                        {
                            //TODO other
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 下载补丁文件
        /// 由于使用www下载，无法获取下载进度。
        /// </summary>
        /// <param name="patchPath"></param>
        public void DownLoadPatchPack(string patchPath)
        {
            string patchPacktUrl = m_UpdateUrl + "/" + patchPath + ".zip";

            OnUpdating(UpdateSegment.DownloadAssets, UpdateError.OK, "download patch pack", 0);
            m_HttpRequest.Get(patchPacktUrl, (err, www) =>
            {
                if (err != null)
                {
                    //获取manifest信息失败
                    OnUpdating(UpdateSegment.DownloadAssets, UpdateError.DownloadPatchPackError, "download fail:" + patchPacktUrl, 1);
                }
                else
                {
                    OnUpdating(UpdateSegment.DownloadAssets, UpdateError.OK, "download patch pack", 1);
                    //save to temp file
                    string localPackFile = Path.Combine(m_StoragePath , patchPath + ".zip");
                    if(!Directory.Exists(Path.GetDirectoryName(localPackFile)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(localPackFile));
                    }
                    File.WriteAllBytes(localPackFile, www.bytes);

                    ApplayPatch(localPackFile);
                }
            },
            (percent)=>
            {
                OnUpdating(UpdateSegment.DownloadAssets, UpdateError.OK, "downloading", percent);
            });
        }

        /// <summary>
        /// 应用补丁
        /// </summary>
        /// <param name="localPakFile"></param>
        protected void ApplayPatch(string localPakFile)
        {
            OnUpdating(UpdateSegment.ApplyAssets, UpdateError.OK, "apply patch", 0);
            using (ZipFile zipFile = new ZipFile(localPakFile))
            {
                bool haveManifest = false;
                Manifest manifest = null;
                Dictionary<string, Asset> assetsMap = null;
                int i = 0;
                foreach (ZipEntry zipEntry in zipFile)
                {
                    if (!haveManifest)
                    {
                        //get manifest
                        haveManifest = true;
                        string manfestContent = null;
                        using (MemoryStream entryContent = new MemoryStream((int)zipEntry.UncompressedSize))
                        {
                            zipEntry.Extract(entryContent);
                            manfestContent = System.Text.Encoding.Default.GetString(entryContent.GetBuffer());
                        }

                        if (!string.IsNullOrEmpty(manfestContent))
                        {
                            manifest = JsonUtility.FromJson<Manifest>(manfestContent);
                            //parse assets
                            assetsMap = ParseManifest(manifest);
                        }
                        else
                        {
                            //error to extract manifest file
                            //stop 
                            break;
                        }
                    }
                    else
                    {
                        OnUpdating(UpdateSegment.ApplyAssets, UpdateError.OK, "apply patch", 0);
                        if (assetsMap!=null && assetsMap.ContainsKey(zipEntry.FileName))
                        {
                            OnUpdating(UpdateSegment.ApplyAssets, UpdateError.OK, "apply patch", (float)(++i)/assetsMap.Count);
                            Asset asset = assetsMap[zipEntry.FileName];
                            switch (asset.type)
                            {
                                case Asset.AssetType.Full:
                                    //extract to the target path
                                    zipEntry.Extract(m_StoragePath, ExtractExistingFileAction.OverwriteSilently);
                                    break;
                                case Asset.AssetType.Patch:
                                    //apply patch
                                    zipEntry.Extract(PatchTempPath, ExtractExistingFileAction.OverwriteSilently);
                                    AllpyPatchFile(zipEntry.FileName);
                                    break;
                               }
                        }
                        else
                        {
                            //the file is not in the manifest ignore
                        }
                    }
                }
            }

            //update CurrentVersion
            UpdateCurrentVersion();

            ClearTempDir();
        }

        protected void AllpyPatchFile(string filename)
        {
            string srcFile = Path.Combine(m_StoragePath, filename);
            string patchFile = Path.Combine(PatchTempPath, filename);
            string outFile = Path.Combine(PatchedPath, filename);
            if(!Directory.Exists(Path.GetDirectoryName(outFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outFile));
            }
            using (FileStream input = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream output = new FileStream(outFile, FileMode.Create))
            {
                BsDiff.BinaryPatchUtility.Apply(input, () => new FileStream(patchFile, FileMode.Open, FileAccess.Read, FileShare.Read), output);
            }

            //copy patched file to source file
            File.Copy(outFile, srcFile, true);
        }

        /// <summary>
        /// 处理Manifest
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns>返回需要进一步操作的资源表</returns>
        protected Dictionary<string,Asset> ParseManifest(Manifest manifest)
        {
            Dictionary<string, Asset> assets = new Dictionary<string, Asset>();
            foreach (Asset asset in manifest.assets)
            {
                switch (asset.type)
                {
                    case Asset.AssetType.Full:
                    case Asset.AssetType.Patch:
                        assets.Add(asset.path, asset);
                        break;
                    case Asset.AssetType.Delete:
                        File.Delete(Path.Combine(m_StoragePath, asset.path));
                        break;
                }
            }
            return assets;
        }

        protected void UpdateCurrentVersion()
        {
            File.WriteAllText(Path.Combine(m_StoragePath, CurrentVersionName),m_CurrentVersion.ToString());
        }

        public string GetPatchPath(Version from,Version to)
        {
            return from.ToString() + "_" + to.ToString();
        }

        public Version GetCurrentVersion()
        {
            Version ver = null;
            if (File.Exists(Path.Combine(m_StoragePath, CurrentVersionName)))
            {
                string content = File.ReadAllText(Path.Combine(m_StoragePath, CurrentVersionName));
                if (!string.IsNullOrEmpty(content) && Version.IsVersionFormat(content.Trim()))
                {
                    ver = new Version(content);
                }
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

        protected void ClearTempDir()
        {
            Directory.Delete(PatchTempPath, true);
            Directory.Delete(PatchedPath, true);
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

        public string StoragePath
        {
            set
            {
                m_StoragePath = value;
            }

            get
            {
                return m_StoragePath;
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

        public string PatchTempPath
        {
            get
            {
                return Path.Combine(m_StoragePath, "_temppatchs");
            }
        }

        public string PatchedPath
        {
            get
            {
                return Path.Combine(m_StoragePath, "_patcheds");
            }
        }
    }
}