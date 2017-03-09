using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

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
        public static string DefaultManifestName = "__.manifest";

        //存放资源的目录，多个版本的资源
        string m_ResourceDir;

        string m_PatchDir;

        //最小版本
        string m_MinVersion;

        //最新版本
        string m_LastVersion;

        /// <summary>
        /// 开始生成补丁
        /// 当版本发布太多的时候，资源目录会包含很多版本文件，这里需要指定fromVersion，而此时的fromVersion也成了这次更新支持的最低版本.
        /// </summary>
        /// <param name="resourceDir">资源目录，各个版本的资源</param>
        /// <param name="fromVersion">从哪个资源开始生成，也就是支持的最低版本。如果不指定，则生成所有.</param>
        /// <param name="toVersion">指明到哪个版本结束，也就是补丁的最新版本。如果不指定，则是最后一个版本。</param>
        public GenerateState Generate(string resourceDir,string patchDir="",string fromVersion="",string toVersion="")
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
            List<Version> versions = GetVertionsInResource(resourceDir);

            //check have version files
            if(versions.Count==0)
            {
                return GenerateState.NoVersions;
            }

            Version minVersion = null;
            Version lastVersion = null;

            //check min version
            if (Version.IsVersionFormat(fromVersion))
            {
                minVersion = new Version(fromVersion); 
            }
            else
            {
                minVersion = versions[0];
            }

            //check current version
            if(Version.IsVersionFormat(toVersion))
            {
                lastVersion = new Version(toVersion);
            }
            else
            {
                lastVersion = versions[versions.Count - 1];
            }

            //记录版本信息
            m_MinVersion = minVersion.ToString();
            m_LastVersion = lastVersion.ToString();
         
            for(int i=0;i<versions.Count;++i)
            {
                Version currentVersion = versions[i];
                if (currentVersion >= minVersion && currentVersion < lastVersion)
                {
                    GenerateBetweenVersion(currentVersion.ToString(), lastVersion.ToString());
                }
            }

            //生成最新的全部文件包。也可以不生成
            GenerateBetweenVersion("", lastVersion.ToString());

            return GenerateState.OK;
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
            Manifest manifest;
            try
            {
                manifest = genManifest.Generate(srcDir, destDir,outDir, "");
                manifest.currentVersion = destVersion;
                manifest.patchVersion = srcVersion;
            }
            catch(System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }            
            
            //out put manifest to json
            EditorUtility.DisplayCancelableProgressBar("Gen Manifest", "Save Manifest",1);
            string manifestJson = JsonUtility.ToJson(manifest);            
            File.WriteAllText(Path.Combine(outDir, DefaultManifestName),manifestJson);
            EditorUtility.ClearProgressBar();
        }

        public void ShowGenerateProgress(GenerateManifest.Segment segment,string msg)
        {
            EditorUtility.DisplayCancelableProgressBar("Gen Manifest", msg, (float)segment / 3);
        }

        public List<Version> GetVertionsInResource(string resoureDir)
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
    }
}
