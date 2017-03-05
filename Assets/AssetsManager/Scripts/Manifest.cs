using System.Collections.Generic;
using System.IO;

namespace YH.AM
{
    [System.Serializable]
    public class Manifest
    {
        /// <summary>
        /// manifest包含的整个资源大小
        /// </summary>
        long m_TotalSize;

        //具体内容
        List<Asset> m_Assets;

        public Manifest()
        {
            m_TotalSize = 0;
            m_Assets = new List<Asset>();
        }

        public void Parse(string file)
        {
            string content="";
            if(File.Exists(file))
            {
                content = File.ReadAllText(file);
            }

            if(!string.IsNullOrEmpty(content))
            {

            }
        }

        public void AddAsset(Asset asset)
        {
            m_Assets.Add(asset);
            m_TotalSize += asset.size;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["totalSize"] = m_TotalSize;

            List<Dictionary<string, object>> assets = new List<Dictionary<string, object>>();
            foreach(Asset asset in m_Assets)
            {
                assets.Add(asset.ToDictionary());
            }
            data["assets"] = assets;
            return data;
        }
    }


}