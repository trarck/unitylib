using System.Collections.Generic;
using System.IO;

namespace YH.Update
{

    [System.Serializable]
    public class ManifestHeader
    {
        public long totalSize;
        public string currentVersion;
        public string patchVersion;
    }

    [System.Serializable]
    public class Manifest
    {
        /// <summary>
        /// manifest包含的整个资源大小
        /// </summary>
        public long totalSize;

        /// <summary>
        /// 当前版本，也是最新版本
        /// </summary>
        public string currentVersion;

        /// <summary>
        /// 补丁版本，表明这个manifest的资源是从补丁版本到当前版本的内容
        /// </summary>
        public string patchVersion;

        //具体内容
        public List<Asset> assets;

        public Manifest()
        {
            totalSize = 0;
            assets = new List<Asset>();
        }

        public void AddAsset(Asset asset)
        {
            assets.Add(asset);
            totalSize += asset.size;
        }

        public ManifestHeader GetHeader()
        {
            ManifestHeader header = new ManifestHeader();
            header.totalSize = totalSize;
            header.currentVersion = currentVersion;
            header.patchVersion = patchVersion;
            return header;
        }
    }
}