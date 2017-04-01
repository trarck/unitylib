using System.Collections.Generic;
using System.IO;

namespace YH.AM
{
    public class RemoteVersions
    {
        public Version LatestVersion;
        public Version MinSupportVersion;
        public Version MinHostVersion;

        public bool Parse(string str)
        {
            //the str is like x.x.x|x.x.x|x.x.x
            string[] items= str.Split('|');
            if (items.Length < 3)
            {
                return false;
            }

            //lastest version
            if(!Version.IsVersionFormat(items[0]))
            {
                return false;
            }
            LatestVersion = new Version(items[0]);

            //min support version
            if (!Version.IsVersionFormat(items[1]))
            {
                return false;
            }
            MinSupportVersion = new Version(items[1]);

            //min host version
            if (!Version.IsVersionFormat(items[2]))
            {
                return false;
            }
            MinHostVersion = new Version(items[2]);

            return true;
        }
    }
}