using System.Collections.Generic;

namespace YH.AM
{
    public class Asset
    {
        public enum AssetType
        {
            //表示整个覆盖。包含新增和修改的。在做资源处理时，直接复制到path相关的路行中。
            //通常只用到size，hash用于校验。
            Full,
            //表示是一个补丁文件，只有对修改的文件起使用。
            //size表示补丁文件大小。
            //hash表示补丁文件的hash.
            //ext表示补丁后的文件的hash用于打完补丁后的验证。但通常不需要。
            Patch,
            //要删除一个资源。hash和size都不用设置
            Delete
        }
        public AssetType type;
        public string path;
        public long size;
        public string hash;
        //留着其它用途
        public string ext;

        public Dictionary<string,object> ToDictionary()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["type"] = type;
            dict["path"] = path;
            dict["size"] = size;
            if(!string.IsNullOrEmpty(hash))
            {
                dict["hash"] = hash;
            }

            if (!string.IsNullOrEmpty(ext))
            {
                dict["ext"] = ext;
            }
            
            return dict;
        }
    }
}
