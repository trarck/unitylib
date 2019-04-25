using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using YH;

namespace YHEditor
{
    public class AutoBuild
    {     
	
        public void Build(BuildTarget target,Dictionary<string,string>options)
        {
            //check build to path
            string outPath = options.ContainsKey("outpath")? options["outpath"]:null;
            if (string.IsNullOrEmpty(outPath))
            {
                EditorUtility.DisplayDialog("build", "no out path", "ok");
                return;
            }

			//parse resource
			ParseResources(target, options);

			//parse plugin.like sdk, lib...
			ParsePlugins(target,options);

			//set project settings
			SetProjectSettings(target,options);      

            //levels to build
            string[] levels = null;
            if (options.ContainsKey("levels"))
            {
                levels = options["levels"].Split(',');
            }
            else
            {
                levels = GetDefaultBuildScenes();
            }

            //build options
            BuildOptions buildOpts = BuildOptions.None;
            if (options.ContainsKey("buildOptionsDevelopment"))
            {
                buildOpts |= BuildOptions.Development;
            }

            if (options.ContainsKey("buildOptionsAllowDebugging"))
            {
                buildOpts |= BuildOptions.AllowDebugging;
            }
            
			//remove exists file or directory
			if(File.Exists(outPath))
			{
				File.Delete(outPath);
			}else if(Directory.Exists(outPath))
			{
                //Directory.Delete(outPath, true);
                FileSystemUtil.ForceDeleteDirectory(outPath);
            }
			
            //call unity build function
            BuildPipeline.BuildPlayer(levels, outPath, target, buildOpts);
        }
		
		void SetProjectSettings(BuildTarget target,Dictionary<string,string>options)
		{
			//set normal 
			SetCommonSettings(options);

			//set special platform 
			Type t = typeof(CmdAutoBuild);
			Debug.Log("cmd:" + "Set" + UcFirst(target.ToString()) + "Settings");
			MethodInfo method = t.GetMethod("Set" + UcFirst(target.ToString()) + "Settings", BindingFlags.NonPublic | BindingFlags.Static);
			if (method != null)
			{
				object[] parameters = new object[1];
				parameters[0] = options;
				method.Invoke(null, parameters);
			}
		}

		void SetCommonSettings(Dictionary<string,string> options)
        {
            //bundleIdentifier
            if (options.ContainsKey("bundleIdentifier"))
            {
                PlayerSettings.applicationIdentifier = options["bundleIdentifier"];
            }

            //bundleVersion
            if (options.ContainsKey("bundleVersion"))
            {
                PlayerSettings.bundleVersion = options["bundleVersion"];
            }

            //stripEngineCode
            if (options.ContainsKey("stripEngineCode"))
            {
                if (options["stripEngineCode"].Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    PlayerSettings.stripEngineCode = true;
                }
                else
                {
                    PlayerSettings.stripEngineCode = false;
                }
            }
        }

        void SetAndroidSettings(Dictionary<string, string> options)
        {
            //版本号
            if (options.ContainsKey("bundleVersionCode"))
            {
                PlayerSettings.Android.bundleVersionCode = int.Parse(options["bundleVersionCode"]);
            }
			else
			{
				//自动加1
				PlayerSettings.Android.bundleVersionCode = PlayerSettings.Android.bundleVersionCode +1;
			}
        }

        void SetIOSSettings(Dictionary<string, string> options)
        {
            //版本号
            if (options.ContainsKey("buildNumber"))
            {
                PlayerSettings.iOS.buildNumber = options["buildNumber"];
            }
			else
			{
				//自动加1
				PlayerSettings.iOS.buildNumber = (int.Parse(PlayerSettings.iOS.buildNumber) + 1).ToString();
			}
        }
		
		void ParseResources(BuildTarget target,Dictionary<string,string>options)
		{
			//build special plateform
			Type t = typeof(CmdAutoBuild);
			MethodInfo method = t.GetMethod("Parse" + UcFirst(target.ToString()) + "Resource", BindingFlags.NonPublic | BindingFlags.Static);
			if (method != null)
			{
				object[] parameters = new object[1];
				parameters[0] = options;
				method.Invoke(null, parameters);
			}
		}

		void ParseAndroidResource(Dictionary<string, string> options)
		{

		}

        void ParseIOSResource(Dictionary<string, string> options)
        {

        }

        void ParsePlugins(BuildTarget target,Dictionary<string, string> options)
		{
            //special platform plugin
			Type t = typeof(CmdAutoBuild);
			MethodInfo method = t.GetMethod("Parse" + UcFirst(target.ToString()) + "Plugins", BindingFlags.NonPublic | BindingFlags.Static);
			if (method != null)
			{
				object[] parameters = new object[1];
				parameters[0] = options;
				method.Invoke(null, parameters);
			}
		}
		
		void ParseAndroidPlugins(Dictionary<string,string> options)
		{
			
		}

        void ParseIOSPlugins(Dictionary<string, string> options)
        {

        }

        static string[] GetDefaultBuildScenes()
        {
            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null)
                    continue;
                if (e.enabled)
                    names.Add(e.path);
            }
            return names.ToArray();
        }

        static string UcFirst(string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }
    }
}