using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Ionic.Zip;

namespace YH.AM
{
    public class GeneratePatch
    {
        public enum GenerateState
        {
            OK,
            NoVersions,
        }

        public static string DefaultPatchDirName = "patch";
        public static string DefaultManifestName = ".manifest";
        public static string VersionFilename = "version.txt";
        public static string MinHostVersionFilename = "minhostversion.txt";

        //存放资源的目录，多个版本的资源
        string m_ResourceDir;

        string m_PatchDir;

        //最小版本
        Version m_MinVersion;

        //最新版本
        Version m_LatestVersion;

        //最小主体版本
        Version m_MinHostVersion;

        //是否生成manifest的头文件
        bool m_GenerateManifestHeader=false;

        //打包否，是否删除补丁文件。
        bool m_RemovePatchsAfterPack = false;

        //是否使用差异补丁
        bool m_UseDiffPatch = false;

        long m_PatchMinFileSize = 128;

        List<string> m_PatchBlackDirs;

        List<string> m_PatchBlackFileExts;

        /// <summary>
        /// 开始生成补丁
        /// 当版本发布太多的时候，资源目录会包含很多版本文件，这里需要指定fromVersion，而此时的fromVersion也成了这次更新支持的最低版本.
        /// </summary>
        /// <param name="resourceDir">资源目录，各个版本的资源</param>
        /// <param name="fromVersion">从哪个资源开始生成，也就是支持的最低版本。如果不指定，则生成所有.</param>
        /// <param name="toVersion">指明到哪个版本结束，也就是补丁的最新版本。如果不指定，则是最后一个版本。</param>
        /// <param name="minHostVersion">主程序的最底版本，就是安装包里的文件版本。一般随安装包变化</param>
        public GenerateState Generate(string resourceDir,string patchDir="",string fromVersion="",string toVersion="",string minHostVersion="")
        {
            m_ResourceDir = resourceDir;

            //检查生成目录，默认放在resource同层。
            if(string.IsNullOrEmpty(patchDir))
            {
                m_PatchDir = Path.Combine(Path.GetDirectoryName(resourceDir), DefaultPatchDirName);
            }
            else
            {
                m_PatchDir = patchDir;
            }

            //开始生成各个版本和最新版本之间的差异。
            List<Version> versions = CheckVersions(resourceDir, fromVersion, toVersion, minHostVersion);
            //check have version files
            if (versions.Count==0)
            {
                return GenerateState.NoVersions;
            }
     
            for(int i=0;i<versions.Count;++i)
            {
                Version currentVersion = versions[i];
                if (currentVersion >= m_MinVersion && currentVersion < m_LatestVersion)
                {
                    GenerateBetweenVersion(currentVersion.ToString(), m_LatestVersion.ToString());
                }
            }

            //生成最新的全部文件包。也可以不生成，超出更新范围的是下载最新的安装文件，也就是和当前最新资源同一个版本。
            GenerateBetweenVersion("", m_LatestVersion.ToString());

            //生成版本信息文件
            GenerateVersionFile(Path.Combine(m_PatchDir, VersionFilename), m_LatestVersion.ToString(), m_MinVersion.ToString(), m_MinHostVersion.ToString());

            return GenerateState.OK;
        }

        protected List<Version> CheckVersions(string resourceDir,string fromVersion, string toVersion , string minHostVersion)
        {
            List<Version> versions = GetVersionsInResource(resourceDir);

            //check have version files
            if (versions.Count == 0)
            {
                return versions;
            }

            Version minVersion = null;
            Version lastVersion = null;

            //check min version
            if (Version.IsVersionFormat(fromVersion))
            {
                m_MinVersion = new Version(fromVersion);
            }
            else
            {
                m_MinVersion = versions[0];
            }

            //check current version
            if (Version.IsVersionFormat(toVersion))
            {
                m_LatestVersion = new Version(toVersion);
            }
            else
            {
                m_LatestVersion = versions[versions.Count - 1];
            }

            //检查最小主体版本
            if (string.IsNullOrEmpty(minHostVersion))
            {
                string minHostVersionFilepath = Path.Combine(resourceDir, MinHostVersionFilename);
                if (File.Exists(minHostVersionFilepath))
                {
                    //get min host version from resource dir
                    minHostVersion = File.ReadAllText(minHostVersionFilepath).Trim();
                    //check is version
                    if (Version.IsVersionFormat(minHostVersion))
                    {
                        m_MinHostVersion = new Version(minHostVersion);
                    }
                    else
                    {
                        m_MinHostVersion = m_MinVersion;
                    }
                }
                else
                {
                    m_MinHostVersion = m_MinVersion;
                }
            }
            return versions;
        }

        protected void GenerateBetweenVersion(string srcVersion,string destVersion)
        {
            Debug.LogFormat("Gen:{0}=>{1}", srcVersion, destVersion);
            //设置目录
            string srcDir = string.IsNullOrEmpty(srcVersion) ? "" : Path.Combine(m_ResourceDir, srcVersion);
            string destDir = string.IsNullOrEmpty(destVersion)?"":Path.Combine(m_ResourceDir, destVersion);
            //输出目录
            string outName = srcVersion;
            if (!string.IsNullOrEmpty(destVersion))
            {
                outName += (string.IsNullOrEmpty(outName)?"":  "_" ) + destVersion;
            }
            string outDir = Path.Combine(m_PatchDir, outName);
            Directory.CreateDirectory(outDir);
            
            //开始生成
            GenerateManifest genManifest = new GenerateManifest();
            genManifest.OnProcessing += ShowGenerateProgress;
            genManifest.useDiffPatch = m_UseDiffPatch;
            if(m_UseDiffPatch)
            {
                AssetBinaryDiffRule rule = new AssetBinaryDiffRule();
                rule.PatchMinFileSize = m_PatchMinFileSize;
                rule.PatchBlackDirs = m_PatchBlackDirs;
                rule.PatchBlackFileExts = m_PatchBlackFileExts;
                genManifest.assetBinaryDiffRule = rule;
            }

            Manifest manifest;
            try
            {
                manifest = genManifest.Generate(srcDir, destDir,outDir, "");
                manifest.currentVersion = destVersion;
                manifest.patchVersion = srcVersion;

                //out put manifest to json
                EditorUtility.DisplayCancelableProgressBar("Save Manifest", Path.Combine(outDir, DefaultManifestName), 0);
                string manifestJson = JsonUtility.ToJson(manifest);
                File.WriteAllText(Path.Combine(outDir, DefaultManifestName), manifestJson);
                EditorUtility.DisplayCancelableProgressBar("Save Manifest", Path.Combine(outDir, DefaultManifestName), 1);
                //pack out patch files
                PackPatchFiles(outDir, DefaultManifestName);
            }
            catch(System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }                       

            EditorUtility.ClearProgressBar();
        }

        //打包生成的补丁文件。要把manifest文件放在第一个。
        public void PackPatchFiles(string patchDir,string manifestFile,string patchFile="")
        {
            EditorUtility.DisplayCancelableProgressBar("Pack", patchDir, 0);

            string manifestContent = File.ReadAllText(Path.Combine(patchDir, manifestFile));

            if (string.IsNullOrEmpty(manifestContent))
            {
                Debug.LogError("The manifest file is empty :" + Path.Combine(patchDir, manifestFile));
                return;
            }

            if (string.IsNullOrEmpty(patchFile))
            {
                patchFile = Path.GetFileName(patchDir) + ".zip";
            }

            if (!Path.IsPathRooted(patchFile))
            {
                patchFile = Path.Combine(Path.GetDirectoryName(patchDir), patchFile);
            }

            Manifest manifest = JsonUtility.FromJson<Manifest>(manifestContent);

            EditorUtility.DisplayCancelableProgressBar("Pack", "zip "+ patchFile, 0.4f);
            using (ZipFile zip = new ZipFile())
            {
                //first add manifest files
                zip.AddFile(Path.Combine(patchDir, manifestFile), "");

                //add other files
                foreach (Asset asset in manifest.assets)
                {
                    zip.AddFile(Path.Combine(patchDir, asset.path), Path.GetDirectoryName(asset.path));
                }

                zip.Save(patchFile);
            }
            EditorUtility.DisplayCancelableProgressBar("Pack", "zip " + patchFile, 1);
            //generate manifest header
            if (m_GenerateManifestHeader)
            {
                string headerFileName = Path.GetFileName(patchDir);
                string headerFile= Path.Combine(Path.GetDirectoryName(patchDir), headerFileName) + ".manifest";
                try
                {
                    string manifestHeaderJson = JsonUtility.ToJson(manifest.GetHeader());
                    File.WriteAllText(headerFile, manifestHeaderJson);
                }
                catch(System.Exception e)
                {
                    throw e;
                }
            }

            //移除生成的patch文件
            if(m_RemovePatchsAfterPack)
            {
                Directory.Delete(patchDir,true);
            }
        }

        //显示生成进度。只有提示做用
        public void ShowGenerateProgress(GenerateManifest.Segment segment,string msg,float percent)
        {
            EditorUtility.DisplayCancelableProgressBar(segment.ToString(), msg, percent);
        }

        /// <summary>
        /// 从目录中提取是版本信息
        /// </summary>
        /// <param name="resoureDir"></param>
        /// <returns></returns>
        public List<Version> GetVersionsInResource(string resoureDir)
        {
            string[] versions = Directory.GetDirectories(resoureDir);
            List<Version> list = new List<Version>();
            //过滤名称是否符合xx.xx.xx的格式
            foreach(string version in versions)
            {
                string versionName = Path.GetFileName(version);
                if(Version.IsVersionFormat(versionName))
                {
                    list.Add(new Version(versionName));
                }
            }

            //按版本号进行排序
            list.Sort((a, b) =>
           {
               return a >= b ? 1 : -1;
           });
            return list;
        }
        
        /// <summary>
        /// 生成版本提示文件
        /// </summary>
        /// <param name="versionFile"></param>
        /// <param name="lastestVersion"></param>
        /// <param name="minSupportVersion"></param>
        /// <param name="minHostVersion"></param>
        public void GenerateVersionFile(string versionFile,string lastestVersion, string minSupportVersion,string minHostVersion="")
        {
            if(string.IsNullOrEmpty(minHostVersion))
            {
                minHostVersion = minSupportVersion;
            }

            string content = lastestVersion + "|" + minSupportVersion + "|" + minHostVersion;
            File.WriteAllText(versionFile, content);
        }

        public bool GenerateManifestHeader
        {
            get
            {
                return m_GenerateManifestHeader;
            }

            set
            {
                m_GenerateManifestHeader = value;
            }
        }

        public bool RemovePatchsAfterPack
        {
            get
            {
                return m_RemovePatchsAfterPack;
            }

            set
            {
                m_RemovePatchsAfterPack = value;
            }
        }

        public bool UseDiffPatch
        {
            get
            {
                return m_UseDiffPatch;
            }

            set
            {
                m_UseDiffPatch = value;
            }
        }

        public long PatchMinFileSize
        {
            set
            {
                m_PatchMinFileSize = value;
            }

            get
            {
                return m_PatchMinFileSize;
            }
        }

        public List<string> PatchBlackDirs
        {
            set
            {
                m_PatchBlackDirs = value;
            }

            get
            {
                return m_PatchBlackDirs;
            }
        }

        public List<string> PatchBlackFileExts
        {
            set
            {
                m_PatchBlackFileExts = value;
            }

            get
            {
                return m_PatchBlackFileExts;
            }
        }
    }
}
