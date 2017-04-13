using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace YH
{
    public class CmdAutoBuild
    {
        public static void Build()
        {
            Dictionary<string, string> options = CmdArguments.GetArgumentOptions();         
            if(options.ContainsKey("buildTarget"))
            {
                //build target
                BuildTarget target = EnumUtil.ParseEnum<BuildTarget>(options["buildTarget"]);
                _Build(target, options);
            }
            else
            {
                EditorUtility.DisplayDialog("build", "no build target", "ok");
            }
            
        }

        public static void BuildAnroid()
        {
            Debug.Log("Build android");
            Dictionary<string, string> options = CmdArguments.GetArgumentOptions();
            _Build(BuildTarget.Android, options);
        }

        static void _Build(BuildTarget target,Dictionary<string,string>options)
        {
            string outPath = options.ContainsKey("outpath")? options["outpath"]:null;
            if (string.IsNullOrEmpty(outPath))
            {
                EditorUtility.DisplayDialog("build", "no out path", "ok");
                return;
            }

            SetCommonSettings(options);

            //call set special platform 
            Type t = typeof(CmdAutoBuild);
            Debug.Log("cmd:" + "Set" + UcFirst(target.ToString()) + "Settings");
            MethodInfo method = t.GetMethod("Set" + UcFirst(target.ToString()) + "Settings", BindingFlags.NonPublic | BindingFlags.Static);
            if( method !=null )
            {
                object[] parameters = new object[1];
                parameters[0] = options;
                method.Invoke(null, parameters);
            }

            //switch (target)
            //{
            //    case BuildTarget.Android:
            //        SetAndroidSettings(options);
            //        break;
            //    case BuildTarget.iOS:
            //        SetIOSSettings(options);
            //        break;
            //    default:
            //        break;
            //}

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
            
            BuildPipeline.BuildPlayer(levels, outPath, target, buildOpts);
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
        }

        static void SetIOSSettings(Dictionary<string, string> options)
        {
            if (options.ContainsKey("buildNumber"))
            {
                PlayerSettings.iOS.buildNumber = options["buildNumber"];
            }
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