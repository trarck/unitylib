using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace YH.Update
{
    public class Version
    {
        static Regex VerRegex = new Regex("(\\d)+(\\.\\d+)*");

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
            Match m = VerRegex.Match(ver);
            if (m != Match.Empty)
            {
                ver = m.Groups[0].Captures[0].Value;

                string[] items = ver.Split('.');

                //major version must have
                if (ushort.TryParse(items[0], out major))
                {
                    if (items.Length > 1)
                    {
                        if (ushort.TryParse(items[1], out minor))
                        {
                            if (items.Length > 2)
                            {
                                ushort.TryParse(items[2], out patch);
                            }
                            else
                            {
                                patch = 0;
                            }
                        }
                    }
                    else
                    {
                        minor = 0;
                        patch = 0;
                    }
                }
            }
            else
            {
                major = 0;
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
            //if (string.IsNullOrEmpty(version))
            //{
            //    return false;
            //}

            //foreach (char c in version)
            //{
            //    if (!char.IsNumber(c) && c != '.')
            //    {
            //        return false;
            //    }
            //}

            return VerRegex.IsMatch(version);
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