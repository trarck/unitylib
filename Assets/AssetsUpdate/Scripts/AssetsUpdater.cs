using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Ionic.Zip;

namespace YH.Update
{
    class AcceptAllCertificatesSignedHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    public class AssetsUpdater : UnitySingleton<AssetsUpdater>
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
            //打补丁错误
            ApplyPatchError,
        }

        public static string CurrentVersionName = "CurrentVersion";

        public static string HostVersionName = "HostVersion";

        //更新地址
        string m_UpdateUrl;

        //本地存储位置
        string m_StoragePath;

        //本地版本
        Version m_CurrentVersion;

        //主体版本。通常存放在可写目录，表示当前补丁对应哪个Host版本。
        Version m_HostVersion;

        //安装包版本
        Version m_NativeHostVersion;

        //主体版本是否在文件中。如果为ture,则由程序启动时写到文件中。
        bool m_HostVersionInFile = true;

        //如果打补丁出错，是否继续打剩下的文件。
        bool m_ContinueWithApplyError = false;

        //是否删除补丁包
        bool m_DeletePatchPack = true;

        bool m_IsInitLocalVersion = false;



        public delegate void UpdatingHandle(UpdateSegment segment, UpdateError err, string msg, float percent);
        public event UpdatingHandle OnUpdating;

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


        HttpClient GetHttpClient()
        {
            HttpClient httpClient = GetComponent<HttpClient>();
            if (!httpClient)
            {
                httpClient = gameObject.AddComponent<HttpClient>();
            }
            return httpClient;
        }
        
        public void GetRemoteVersion()
        {
            System.DateTime now = System.DateTime.Now;
            string remoteVersionUrl = m_UpdateUrl + "/version.txt?t="+ now.Ticks;
            TriggerUpdating(UpdateSegment.CompareVersion, UpdateError.OK, "Get remote version", 0);

            HttpClient httpClient = GetHttpClient();
            //makesure the client is clean
            httpClient.Clear();
            httpClient.onComplete += (_) =>
            {
                if (httpClient.request.error != null)
                {
                    //获取远程版本信息失败
                    TriggerUpdating(UpdateSegment.CompareVersion, UpdateError.DownloadRemoteVersionError, "Download remote version " + remoteVersionUrl + " fail:" + httpClient.request.error, 1);
                }
                else
                {
                    //获取远程版本信息成功
                    TriggerUpdating(UpdateSegment.CompareVersion, UpdateError.OK, "Get remote version", 0.2f);
                    YH.Log.YHDebug.LogFormat("Remove version:{0}", httpClient.request.downloadHandler.text);
                    RemoteVersions remoteVersions = new RemoteVersions();
                    if (remoteVersions.Parse(httpClient.request.downloadHandler.text))
                    {
                        //比较版本
                        CompareVersion(remoteVersions); //Step
                    }
                    else
                    {
                        TriggerUpdating(UpdateSegment.CompareVersion, UpdateError.ParseRemoteVersionError, "Parse remote version fail:" + httpClient.request.downloadHandler.text, 1);
                    }
                }
            };

            httpClient.Get(remoteVersionUrl);
        }

        public void InitLocalVersion()
        {
            if (m_IsInitLocalVersion) return;

            m_IsInitLocalVersion = true;

            //get local version
            m_CurrentVersion = GetCurrentVersion();
            m_HostVersion = GetHostVersion();
            m_NativeHostVersion = GetNativeHostVersion();

            if (m_HostVersion == null)
            {
                m_HostVersion = m_NativeHostVersion;
            }

            //bool haveLocalCurrentVersion = true;
            if (m_CurrentVersion == null)
            {
                m_CurrentVersion = m_NativeHostVersion;
                //haveLocalCurrentVersion = false;
            }
        }

        protected void CompareVersion(RemoteVersions remoteVersions)
        {
            //make sure local version is set.
            InitLocalVersion();

            //check host version and native version
            //1.m_HostVersion==m_NativeHostVersion                        same install package or no local HostVersion file.
            //    when m_CurrentVersion < m_NativeHostVersion        Update from NativeHostVersion
            //2.m_HostVersion<m_NativeHostVersion                         user remove install package and reinstall new version package.
            //   when m_CurrentVersion < m_NativeHostVersion        Update from NativeHostVersion.Others use currentVersion for update.
            //3.m_HostVersion > m_NativeHostVersion       user remove install package and reinstall old version package. 
            //                                                                          must update from native host version.     
            if ((m_HostVersion <= m_NativeHostVersion && m_CurrentVersion < m_NativeHostVersion) || m_HostVersion > m_NativeHostVersion)
            {
                //Update from NativeHostVersion
                m_HostVersion = m_NativeHostVersion;
                m_CurrentVersion = m_NativeHostVersion;
                ClearStorageDir();
                WriteHostVersionToFile();
            }

            //if (m_HostVersion < m_NativeHostVersion)
            //{
            //    //user remove install package and reinstall new version package.
            //    if(m_CurrentVersion<m_NativeHostVersion)
            //    {
            //        //Update from NativeHostVersion
            //        m_HostVersion = m_NativeHostVersion;
            //        m_CurrentVersion = m_NativeHostVersion;
            //        ClearStorageDir();
            //        WriteHostVersionToFile();
            //    }              
            //}
            //else if (m_HostVersion > m_NativeHostVersion)
            //{
            //    //user remove install package and reinstall old version package. must update from native host version.               
            //    m_HostVersion = m_NativeHostVersion;
            //    m_CurrentVersion = m_NativeHostVersion;
            //    ClearStorageDir();
            //    WriteHostVersionToFile();
            //}

            //first check the host is support
            if (remoteVersions.MinHostVersion > m_NativeHostVersion)
            {
                //the host is out data
                TriggerUpdating(UpdateSegment.CompareVersion, UpdateError.HostIsOutOfDate, "Please download the newest application.", 1);
                return;
            }

            //check min support version
            if (remoteVersions.MinSupportVersion > m_CurrentVersion)
            {
                TriggerUpdating(UpdateSegment.CompareVersion, UpdateError.CurrentVersionOutOfDate, "Please download the newest application.", 1);
                return;
            }

            //check is latest version
            if (m_CurrentVersion >= remoteVersions.LatestVersion)
            {
                //the current is latest version,complete the update
                TriggerUpdating(UpdateSegment.Complete, UpdateError.OK, "The assets is latest did not need update.", 1);
                return;
            }

            TriggerUpdating(UpdateSegment.CompareVersion, UpdateError.OK, "Compare complete.", 1);

            //do update
            string patchPath = GetPatchPath(m_CurrentVersion, remoteVersions.LatestVersion);
            string allPatch = remoteVersions.LatestVersion.ToString();
            //TODO download the manifest file.
            //GetManifestHeaderFile(patchPath);
            //download the pack file            
            DownLoadPatchPack(patchPath, allPatch);//Step

            //更新当前版本号，但不保存到文件.等所有补丁执行完成才保存到本地文件。
            m_CurrentVersion = remoteVersions.LatestVersion;
        }

        public void GetManifestHeaderFile(string patchPath)
        {
            string manifestUrl = m_UpdateUrl + "/" + patchPath + ".manifest";

            HttpClient httpClient = GetHttpClient();
            //makesure the client is clean
            httpClient.Clear();
            httpClient.onComplete += (_) =>
            {
                if (httpClient.request.error != null)
                {
                    //获取manifest信息失败
                    TriggerUpdating(UpdateSegment.DownloadAssets, UpdateError.DownloadManifestError, "Download manifest " + manifestUrl + " fail:" + httpClient.request.error, 1);
                }
                else
                {
                    if (string.IsNullOrEmpty(httpClient.request.downloadHandler.text))
                    {
                        TriggerUpdating(UpdateSegment.DownloadAssets, UpdateError.DownloadManifestError, "Manifest file is empty:" + manifestUrl, 1);
                    }
                    else
                    {
                        ManifestHeader manifestHeader = JsonUtility.FromJson<ManifestHeader>(httpClient.request.downloadHandler.text);
                        if (manifestHeader == null)
                        {
                            TriggerUpdating(UpdateSegment.DownloadAssets, UpdateError.DownloadManifestError, "Parse manifest fail:" + httpClient.request.downloadHandler.text, 1);
                        }
                        else
                        {
                            //TODO other
                        }
                    }
                }
            };

            httpClient.Get(manifestUrl);
        }

        /// <summary>
        /// 下载补丁文件
        /// 由于使用www下载，无法获取下载进度。
        /// </summary>
        /// <param name="patchPath"></param>
        public void DownLoadPatchPack(string patchPath,string allPatch)
        {
            string patchPacktUrl = m_UpdateUrl + "/" + patchPath + ".zip";

            YH.Log.YHDebug.LogFormat("patch pack url:{0}", patchPacktUrl);
            TriggerUpdating(UpdateSegment.DownloadAssets, UpdateError.OK, "Download patch pack", 0);

            bool loadPatchAll = false;

            HttpClient httpClient = GetHttpClient();
            //makesure the client is clean
            httpClient.Clear();
            httpClient.onComplete += (_) =>
            {
                if (httpClient.request.error != null)
                {
                    if (!loadPatchAll)
                    {
                        loadPatchAll = true;
                        string allPatchPacktUrl = m_UpdateUrl + "/" + patchPath + ".zip";
                        YH.Log.YHDebug.LogFormat("download patch pack fail download all:{0}", allPatchPacktUrl);
                        httpClient.Get(allPatchPacktUrl);
                    }
                    else
                    {
                        //获取manifest信息失败
                        TriggerUpdating(UpdateSegment.DownloadAssets, UpdateError.DownloadPatchPackError, "Download patch pack " + patchPacktUrl + " fail:" + httpClient.request.error, 1);
                    }
                }
                else
                {
                    TriggerUpdating(UpdateSegment.DownloadAssets, UpdateError.OK, "Download patch pack", 1);
                    //save to temp file
                    string localPackFile = Path.Combine(m_StoragePath, patchPath + ".zip");
                    if (!Directory.Exists(Path.GetDirectoryName(localPackFile)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(localPackFile));
                    }

                    MemoryStream stream = new MemoryStream(httpClient.request.downloadHandler.data);
                    StartCoroutine(ApplayPatch(stream));

                    if (!m_DeletePatchPack)
                    {
                        File.WriteAllBytes(localPackFile, httpClient.request.downloadHandler.data);
                    }
                }
                //release request
                httpClient.Clear();
            };

            httpClient.onProgress += (percent) =>
            {
                TriggerUpdating(UpdateSegment.DownloadAssets, UpdateError.OK, "Downloading", percent);
            };
            httpClient.Get(patchPacktUrl);
        }

        /// <summary>
        /// 应用补丁。不使用了，
        /// </summary>
        /// <param name="localPakFile"></param>
//        protected IEnumerator ApplayPatch(string localPakFile)
//        {
//            TriggerUpdating(UpdateSegment.ApplyAssets, UpdateError.OK, "apply patch", 0);
//            bool haveError = false;
//            using (ZipFile zipFile = new ZipFile(localPakFile, System.Text.Encoding.UTF8))
//            {
//                bool haveManifest = false;
//                Manifest manifest = null;
//                Dictionary<string, Asset> assetsMap = null;
//                int i = 0;
//                foreach (ZipEntry zipEntry in zipFile)
//                {
//                    if (!haveManifest)
//                    {
//                        //get manifest
//                        haveManifest = true;
//                        string manfestContent = null;
//                        using (MemoryStream entryContent = new MemoryStream((int)zipEntry.UncompressedSize))
//                        {
//                            zipEntry.Extract(entryContent);
//                            manfestContent = System.Text.Encoding.Default.GetString(entryContent.GetBuffer());
//                        }

//                        if (!string.IsNullOrEmpty(manfestContent))
//                        {
//                            manifest = JsonUtility.FromJson<Manifest>(manfestContent);
//                            //parse assets
//                            assetsMap = ParseManifest(manifest);
//                        }
//                        else
//                        {
//                            //error to extract manifest file
//                            //stop 
//                            break;
//                        }
//                    }
//                    else
//                    {
//                        if (assetsMap != null && assetsMap.ContainsKey(zipEntry.FileName))
//                        {
//                            Asset asset = assetsMap[zipEntry.FileName];
//                            switch (asset.type)
//                            {
//                                case Asset.AssetType.Full:
//                                    //extract to the target path
//                                    zipEntry.Extract(m_StoragePath, ExtractExistingFileAction.OverwriteSilently);
//                                    TriggerUpdating(UpdateSegment.ApplyAssets, UpdateError.OK, "apply patch", (float)(++i) / assetsMap.Count);
//                                    break;
//                                case Asset.AssetType.Patch:
//                                    //apply patch
//#if USE_BSDIFF
//                                    zipEntry.Extract(PatchTempPath, ExtractExistingFileAction.OverwriteSilently);
//                                    if (AllpyPatchFile(zipEntry.FileName))
//                                    {
//                                        TriggerUpdating(UpdateSegment.ApplyAssets, UpdateError.OK, "apply patch", (float)(++i) / assetsMap.Count);
//                                    }
//                                    else
//                                    {
//                                        TriggerUpdating(UpdateSegment.ApplyAssets, UpdateError.ApplyPatchError, "patch fail file:" + zipEntry.FileName, (float)(++i) / assetsMap.Count);
//                                        haveError = true;
//                                    }
//#endif
//                                    break;
//                            }
//                            //stop 
//                            if (!m_ContinueWithApplyError && haveError)
//                            {
//                                break;
//                            }
//                            yield return new WaitForEndOfFrame();
//                        }
//                        else
//                        {
//                            //the file is not in the manifest ignore
//                        }
//                    }
//                }
//            }

//            //update CurrentVersion
//            if (!haveError)
//            {
//                WriteCurrentVersionToFile();
//            }

//            DeleteTempDir();

//            if (m_DeletePatchPack && File.Exists(localPakFile))
//            {
//                File.Delete(localPakFile);
//            }

//            TriggerUpdating(UpdateSegment.Complete, UpdateError.OK, "update complete", 1);
//        }

        /// <summary>
        /// 应用补丁。
        /// 保存到文件后再解压，没有从内存中直接解压同时保存文件快。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected IEnumerator ApplayPatch(Stream stream)
        {
            TriggerUpdating(UpdateSegment.ApplyAssets, UpdateError.OK, "Apply patch", 0);
            bool haveError = false;
            using (ZipFile zipFile = ZipFile.Read(stream))
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
                        if (assetsMap != null && assetsMap.ContainsKey(zipEntry.FileName))
                        {
                            Asset asset = assetsMap[zipEntry.FileName];
                            switch (asset.type)
                            {
                                case Asset.AssetType.Full:
                                    //extract to the target path
                                    zipEntry.Extract(m_StoragePath, ExtractExistingFileAction.OverwriteSilently);
                                    TriggerUpdating(UpdateSegment.ApplyAssets, UpdateError.OK, "Apply patch", (float)(++i) / assetsMap.Count);
                                    break;
                                case Asset.AssetType.Patch:
                                    //apply patch
#if USE_BSDIFF
                                    zipEntry.Extract(PatchTempPath, ExtractExistingFileAction.OverwriteSilently);
                                    if (AllpyPatchFile(zipEntry.FileName))
                                    {
                                        TriggerUpdating(UpdateSegment.ApplyAssets, UpdateError.OK, "Apply patch", (float)(++i) / assetsMap.Count);
                                    }
                                    else
                                    {
                                        TriggerUpdating(UpdateSegment.ApplyAssets, UpdateError.ApplyPatchError, "Patch fail file:" + zipEntry.FileName, (float)(++i) / assetsMap.Count);
                                        haveError = true;
                                    }
#endif
                                    break;
                            }
                            //stop 
                            if (!m_ContinueWithApplyError && haveError)
                            {
                                break;
                            }
                            yield return new WaitForEndOfFrame();
                        }
                        else
                        {
                            //the file is not in the manifest ignore
                        }
                    }
                }
            }

            //update CurrentVersion
            if (!haveError)
            {
                WriteCurrentVersionToFile();
            }

            DeleteTempDir();

            stream.Dispose();

            TriggerUpdating(UpdateSegment.Complete, UpdateError.OK, "Update complete", 1);
        }

        protected bool AllpyPatchFile(string filename)
        {
#if USE_BSDIFF
            string srcFile = Path.Combine(m_StoragePath, filename);
            if (!File.Exists(srcFile))
            {
                return false;
            }

            string patchFile = Path.Combine(PatchTempPath, filename);
            string outFile = Path.Combine(PatchedPath, filename);
            if (!Directory.Exists(Path.GetDirectoryName(outFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outFile));
            }
            using (FileStream input = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream output = new FileStream(outFile, FileMode.Create))
            {
                BinaryPatchUtility.Apply(input, () => new FileStream(patchFile, FileMode.Open, FileAccess.Read, FileShare.Read), output);
            }

            //copy patched file to source file
            File.Copy(outFile, srcFile, true);
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// 处理Manifest
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns>返回需要进一步操作的资源表</returns>
        protected Dictionary<string, Asset> ParseManifest(Manifest manifest)
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
                        string deleteFilePath = Path.Combine(m_StoragePath, asset.path);
                        if (File.Exists(deleteFilePath))
                            File.Delete(deleteFilePath);
                        break;
                }
            }
            return assets;
        }

        public string GetPatchPath(Version from, Version to)
        {
            return from.ToString() + "_" + to.ToString();
        }

        protected void WriteCurrentVersionToFile()
        {
            File.WriteAllText(Path.Combine(m_StoragePath, CurrentVersionName), m_CurrentVersion.ToString());
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


        protected void WriteHostVersionToFile()
        {
            File.WriteAllText(Path.Combine(m_StoragePath, HostVersionName), m_HostVersion.ToString());
        }

        public Version GetHostVersion()
        {
            Version ver = null;
            if (File.Exists(Path.Combine(m_StoragePath, HostVersionName)))
            {
                string content = File.ReadAllText(Path.Combine(m_StoragePath, HostVersionName));
                if (!string.IsNullOrEmpty(content) && Version.IsVersionFormat(content.Trim()))
                {
                    ver = new Version(content);
                }
            }
            return ver;
        }

        public Version GetNativeHostVersion()
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
                OnUpdating(segment, err, msg, percent);
        }

        protected void DeleteTempDir()
        {
            if (Directory.Exists(PatchTempPath))
                ForceDeleteDirectory(PatchTempPath);
            if (Directory.Exists(PatchedPath))
                ForceDeleteDirectory(PatchedPath);
        }

        protected void ClearStorageDir()
        {
            if (Directory.Exists(m_StoragePath))
                ForceCelarDirectory(m_StoragePath);
        }

        public static void ForceDeleteDirectory(string path)
        {
            DirectoryInfo root;
            Stack<DirectoryInfo> fols;
            DirectoryInfo fol;
            fols = new Stack<DirectoryInfo>();
            root = new DirectoryInfo(path);
            fols.Push(root);
            while (fols.Count > 0)
            {
                fol = fols.Peek();
                fol.Attributes = fol.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);

                foreach (FileInfo f in fol.GetFiles())
                {
                    f.Attributes = f.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                    f.Delete();
                }

                DirectoryInfo[] subs = fol.GetDirectories();
                if (subs.Length > 0)
                {
                    foreach (DirectoryInfo d in fol.GetDirectories())
                    {
                        fols.Push(d);
                    }
                }
                else
                {
                    fol.Delete(true);
                    fols.Pop();
                }
            }
        }

        public static void ForceCelarDirectory(string path)
        {
            DirectoryInfo root;
            Stack<DirectoryInfo> fols;
            DirectoryInfo fol;
            fols = new Stack<DirectoryInfo>();
            root = new DirectoryInfo(path);
            fols.Push(root);
            while (fols.Count > 0)
            {
                fol = fols.Peek();
                fol.Attributes = fol.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);

                foreach (FileInfo f in fol.GetFiles())
                {
                    f.Attributes = f.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                    f.Delete();
                }

                DirectoryInfo[] subs = fol.GetDirectories();
                if (subs.Length > 0)
                {
                    foreach (DirectoryInfo d in fol.GetDirectories())
                    {
                        fols.Push(d);
                    }
                }
                else
                {
                    if (fol != root)
                    {
                        fol.Delete(true);
                    }

                    fols.Pop();
                }
            }
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

        public Version CurrentVersion
        {
            get
            {
                return m_CurrentVersion;
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

        public bool IsContinueWithApplyError
        {
            set
            {
                m_ContinueWithApplyError = value;
            }

            get
            {
                return m_ContinueWithApplyError;
            }
        }

        public bool DeletePatchPack
        {
            set
            {
                m_DeletePatchPack = value;
            }

            get
            {
                return m_DeletePatchPack;
            }
        }
    }
}