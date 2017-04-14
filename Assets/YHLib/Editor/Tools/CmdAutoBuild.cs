using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace YH
{
    public class CmdAutoBuild
    {     
        public static void BuildAnroid()
        {
            Debug.Log("Build android");
            Dictionary<string, string> options = CmdArguments.GetArgumentOptions();
            Build(BuildTarget.Android, options);
        }

		public static void Build()
        {
            Dictionary<string, string> options = CmdArguments.GetArgumentOptions();         
            if(options.ContainsKey("buildTarget"))
            {
                //build target
                BuildTarget target = EnumUtil.ParseEnum<BuildTarget>(options["buildTarget"]);
                Build(target, options);
            }
            else
            {
                EditorUtility.DisplayDialog("build", "no build target", "ok");
            }            
        }
		
        public static void Build(BuildTarget target,Dictionary<string,string>options)
        {
            string outPath = options.ContainsKey("outpath")? options["outpath"]:null;
            if (string.IsNullOrEmpty(outPath))
            {
                EditorUtility.DisplayDialog("build", "no out path", "ok");
                return;
            }

			//parse resource
			ParseResources(target, options);

			//parse plugin. sdk,lib...
			ParsePlugins(target,options);

			//set project settings
			SetProjectSettings(target,options);      

            //levels
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
            
			//remove exists file
			if(File.Exists(outPath))
			{
				File.Delete(outPath);
			}else if(Directory.Exists(outPath))
			{
				Directory.Delete(outPath, true);
			}
			
            BuildPipeline.BuildPlayer(levels, outPath, target, buildOpts);
        }
		
		static void SetProjectSettings(BuildTarget target,Dictionary<string,string>options)
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

		static void SetCommonSettings(Dictionary<string,string> options)
        {
            if (options.ContainsKey("bundleIdentifier"))
            {
                PlayerSettings.bundleIdentifier = options["bundleIdentifier"];
            }

            if (options.ContainsKey("bundleVersion"))
            {
                PlayerSettings.bundleVersion = options["bundleVersion"];
            }
        }

        static void SetAndroidSettings(Dictionary<string, string> options)
        {
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

        static void SetIOSSettings(Dictionary<string, string> options)
        {
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
		
		static void ParseResources(BuildTarget target,Dictionary<string,string>options)
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

		static void BuildAndroidResource(Dictionary<string, string> options)
		{

		}
	
		static void ParsePlugins(BuildTarget target,Dictionary<string, string> options)
		{
			//libs

			//sdks
			Type t = typeof(CmdAutoBuild);
			MethodInfo method = t.GetMethod("Parse" + UcFirst(target.ToString()) + "Plugins", BindingFlags.NonPublic | BindingFlags.Static);
			if (method != null)
			{
				object[] parameters = new object[1];
				parameters[0] = options;
				method.Invoke(null, parameters);
			}
		}
		
		static void ParseAndroidPlugins(Dictionary<string,string> options)
		{
			string sdk = options.ContainsKey("sdk")? options["sdk"]:"Base";
			string sdkPath=Application.dataPath+"../PackTools/"+sdk;

			EditorHelper.DeleteFolder(AutoPack.AndroidPluginPath);
			
			EditorHelper.CopyDirectory(sdkPath, AutoPack.AndroidPluginPath);
			
			AssetDatabase.Refresh();
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

        [MenuItem("MyMenu/Test Cmd Build")]
        public static void TestCmbBuild()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("outpath", "d:\\temp\\a\\a.apk");
            dict.Add("bundleVersion", "1.0.6");
            dict.Add("bundleVersionCode", "6");
            CmdAutoBuild._Build(BuildTarget.Android, dict);
        }
    }
}