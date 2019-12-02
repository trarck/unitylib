using System.Collections.Generic;
using System.IO;

namespace YH.Update
{
    public class Version
    {
        public ushort major=0;
        public ushort minor =0;
        public ushort patch =0;
        
        public Version()
        {

        }   
           
        public Version(string ver)
        {
            Parse(ver);
        }

        /// <summary>
        /// version:
        /// a.b.c            major=a,minor=b,patch=3
        /// a.b               major=a,minor=b,patch=0
        /// a                  major=a,minor=0,patch=0    
        /// </summary>
        /// <param name="ver"></param>
        public void Parse(string ver)
        {
            string[] items = ver.Split('.');

            //major version must have
            major = ushort.Parse(items[0]);

            if(items.Length>1)
            {
                minor = ushort.Parse(items[1]);

                if(items.Length>2)
                {
                    patch = ushort.Parse(items[2]);
                }
                else
                {
                    patch = 0;
                }
            }
            else
            {
                minor = 0;
                patch = 0;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", major, minor, patch);
        }

        public ulong ToLong()
        {
            return  ((ulong)major << 32) | ((ulong)minor << 16) | patch;
        }

        public static bool IsVersionFormat(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return false;
            }

            foreach (char c in version)
            {
                if (!char.IsNumber(c) && c != '.')
                {
                    return false;
                }
            }

            return true;
        }


        //==================overload compare operator=====================//
        public static bool operator ==(Version lhs,Version rhs)
        {
            if(object.Equals(lhs,null) || object.Equals(rhs,null))
            {
                return object.Equals(lhs, rhs);
            }
            return lhs.ToLong() == rhs.ToLong();
        }

        public static bool operator !=(Version lhs, Version rhs)
        {
            if (object.Equals(lhs, null) || object.Equals(rhs, null))
            {
                return !object.Equals(lhs, rhs);
            }
            return lhs.ToLong() != rhs.ToLong();
        }

        public static bool operator >(Version lhs, Version rhs)
        {
            return lhs.ToLong() > rhs.ToLong();
        }

        public static bool operator <(Version lhs, Version rhs)
        {
            return lhs.ToLong() < rhs.ToLong();
        }

        public static bool operator >=(Version lhs, Version rhs)
        {
            return lhs.ToLong() >= rhs.ToLong();
        }

        public static bool operator <=(Version lhs, Version rhs)
        {
            return lhs.ToLong() <= rhs.ToLong();
        }
    }
}