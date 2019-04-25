using UnityEngine;
using System.Collections.Generic;
using YH;

namespace YHEditor
{
    public class CmdArguments
    {
        public static string ProjectArgPrefix = "yh-";

        public static Dictionary<string, string> GetArgumentOptions()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();

            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith(ProjectArgPrefix))
                {
                    string cnt = arg.Substring(ProjectArgPrefix.Length);
                    if (cnt.IndexOf("=") > 0)
                    {
                        string[] itmes = cnt.Split('=');
                        args.Add(itmes[0], itmes[1]);
                    }
                }
            }

            return args;
        }

        public static List<string> GetArgumentCmds()
        {
            List<string> cmds = new List<string>();

            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith(ProjectArgPrefix))
                {
                    string cmd = arg.Substring(ProjectArgPrefix.Length);
                    if (cmd.IndexOf("=") == -1)
                    {
                        cmds.Add(cmd);
                    }
                }
            }

            return cmds;
        }
    }
}