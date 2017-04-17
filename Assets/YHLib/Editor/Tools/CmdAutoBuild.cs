using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;

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

        public static void Build(BuildTarget target, Dictionary<string, string> options)
        {
            AutoBuild autoBuild = new AutoBuild();
            autoBuild.Build(target, options);
        }

        [MenuItem("MyMenu/Test Cmd Build")]
        public static void TestCmbBuild()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("outpath", "d:\\temp\\a\\a.apk");
            dict.Add("bundleVersion", "1.0.6");
            dict.Add("bundleVersionCode", "6");
            CmdAutoBuild.Build(BuildTarget.Android, dict);
        }
    }
}