using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace YH.AM
{
    public class GenerateManifest
    {
        //保存新增的文件路径
        List<string> m_Addes = new List<string>();
        //保存删除的文件路径
        List<string> m_Deletes = new List<string>();
        //保存更改的文件路径
        List<string> m_Modifies = new List<string>();

        //源目录
        string m_srcPath;

        //目标目录
        string m_destPath;

        //是否使用差异补丁
        public bool useDiffPatch=false;

        public struct DirData
        {
            public string srcPath;
            public string destPath;
            public string dirName;

            public DirData(string _srcPath, string _destPath, string _dirname)
            {
                srcPath = _srcPath;
                destPath = _destPath;
                dirName = _dirname;
            }
        }

        public enum Segment
        {
            Process,
            Manifest,
            Collect
        }

        //处理阶段信息回调
        public delegate void ProcessHandle(Segment segment,string msg,float percent);
        public ProcessHandle OnProcessing;

        public Manifest Generate(string srcDir, string destDir, string outDir,string dirName = "")
        {
            //处理目录，生成差异文件
            Process(srcDir, destDir, dirName);

            //由差差异文件生成Manifest
            Manifest manifest= GetManifest();

            //收集Manifest的资源
            CollectAssets(manifest,outDir);
            return manifest;
        }

        public void Process(string srcDir, string destDir, string dirName = "")
        {
            //保存下来，后面获取具体信息时有用。
            m_srcPath = srcDir;
            m_destPath = destDir;
            //如果目录目录是空，则什么都不做。没有源目录，难道要删除所有文件？
            if (string.IsNullOrEmpty(destDir))
            {
                return;
            }

            //检查目标目录是否存在
            if (string.IsNullOrEmpty(srcDir))
            {
                //没有目标目录，则添加所有源目录。
                AddAllFiles(destDir, dirName, m_Addes);
            }
            else
            {
                Queue<DirData> dirs = new Queue<DirData>();
                dirs.Enqueue(new DirData(srcDir, destDir, dirName));
                while (dirs.Count > 0)
                {
                    DirData dirData = dirs.Dequeue();
                    ProcessFiles(dirData.srcPath, dirData.destPath, dirData.dirName);
                    ProcessSubDirs(dirData.srcPath, dirData.destPath, dirData.dirName, dirs);
                }
            }
        }

        public void ProcessFiles(string srcDir, string destDir, string dirName)
        {
            string[] srcFiles = Directory.GetFiles(srcDir);

            string srcPath;
            string destPath;
            Dictionary<string, bool> fileSigns = new Dictionary<string, bool>();
            OnProcessing(Segment.Process, "process dir " + srcDir, 0);
            foreach (string file in srcFiles)
            {
                string fileName = Path.GetFileName(file);
                srcPath = Path.Combine(srcDir, fileName);
                destPath = Path.Combine(destDir, fileName);

                //如果目标文件不存在，则表示源文件被删除
                if (!File.Exists(destPath))
                {
                    m_Deletes.Add(JoinPath(dirName, fileName));
                }
                //如果和目标文件不一致，表示已经被修改
                else if (!CompareFile(srcPath, destPath))
                {
                    m_Modifies.Add(JoinPath(dirName, fileName));
                }

                fileSigns[fileName] = true;

                FireProcessing(Segment.Process, "process fire " + file,1);
            }

            //检查目标目录的文件
            string[] destFiles = Directory.GetFiles(destDir);
            foreach (string file in destFiles)
            {
                string fileName = Path.GetFileName(file);
                //如果文件没有标记过，表示是新增加的文件
                if (!fileSigns.ContainsKey(fileName))
                {
                    m_Addes.Add(dirName + "/" + fileName);
                }

                FireProcessing(Segment.Process, "process fire " + file, 1);
            }            
        }

        public void ProcessSubDirs(string srcDir, string destDir, string dirName, Queue<DirData> dirs)
        {
            string[] srcSubDirs = Directory.GetDirectories(srcDir);

            string srcPath;
            string destPath;
            Dictionary<string, bool> dirSigns = new Dictionary<string, bool>();
            foreach (string dir in srcSubDirs)
            {
                string subName = Path.GetFileName(dir);
                srcPath = Path.Combine(srcDir, subName); ;
                destPath = Path.Combine(destDir, subName);

                //如果目标文件夹不存在，则表示源文夹整个被删除
                if (!Directory.Exists(destPath))
                {
                    //删除整个目录，不用处理子目录的文件，会被一起删除。
                    m_Deletes.Add(JoinPath(dirName, subName));
                }
                else
                {
                    //处理子文件夹
                    //把要继续要处理的文件加入队列。这里不使用递归。
                    dirs.Enqueue(new DirData(srcPath, destPath, JoinPath(dirName, subName)));
                }
                dirSigns[subName] = true;
            }

            //检查目标目录的文件夹
            string[] destSubDirs = Directory.GetDirectories(destDir);
            foreach (string dir in destSubDirs)
            {
                string subName = Path.GetFileName(dir);
                //如果文件夹没有标记过，表示是新增加的文件夹
                if (!dirSigns.ContainsKey(subName))
                {
                    AddAllFiles(Path.Combine(destDir, dir), JoinPath(dirName, subName), m_Addes);
                }
            }
        }

        /// <summary>
        /// 目录下的所有文件（包含子目录下的）添加到列表
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="dirName"></param>
        /// <param name="list"></param>
        void AddAllFiles(string dirPath, string dirName, List<string> list)
        {
            //记录所有要增加到，增加列表的目录
            Queue<DirData> dirs = new Queue<DirData>();
            dirs.Enqueue(new DirData(dirPath, "", dirName));

            while (dirs.Count > 0)
            {
                DirData dirData = dirs.Dequeue();
                //添加该目录下的所有文件
                string[] files = Directory.GetFiles(dirData.srcPath);
                foreach (string file in files)
                {
                    list.Add(JoinPath(dirData.dirName, Path.GetFileName(file)));
                }

                //add sub dirs
                string[] subDirs = Directory.GetDirectories(dirData.srcPath);
                foreach (string subDir in subDirs)
                {
                    dirs.Enqueue(new DirData(subDir, "", JoinPath(dirData.dirName, Path.GetFileName(subDir))));
                }
            }
        }

        /// <summary>
        /// 比较二个文件是否一样
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns>相同为true，否则为false</returns>
        public bool CompareFile(string src, string dest)
        {
            //first check file size
            FileInfo srcInfo = new FileInfo(src);
            FileInfo destInfo = new FileInfo(dest);
            if (srcInfo.Length != destInfo.Length)
            {
                return false;
            }

            //then check file hash.now use md5
            FileStream srcStream = null;
            FileStream destStream = null;
            bool ret = false;
            try
            {
                srcStream = srcInfo.OpenRead();
                destStream = destInfo.OpenRead();
                ret = CheckFileHash(srcStream, destStream);
            }
            finally
            {
                if (srcStream != null)
                {
                    srcStream.Close();
                }

                if (destStream != null)
                {
                    destStream.Close();
                }
            }

            return ret;
        }

        /// <summary>
        /// 检查二个文件流的hash值是否一样
        /// </summary>
        /// <param name="srcStream"></param>
        /// <param name="destStream"></param>
        /// <returns>true 一样，false 不一样</returns>
        public bool CheckFileHash(FileStream srcStream, FileStream destStream)
        {
            //create md5 hasher
            MD5 md5Hasher = MD5.Create();
            //calc src stream hash data
            byte[] srcData = md5Hasher.ComputeHash(srcStream);
            //calc dest stream hash data
            byte[] destData = md5Hasher.ComputeHash(destStream);

            //compare hash data size
            if (srcData.Length != destData.Length)
            {
                return false;
            }

            //compare hash data content
            for (int i = 0; i < srcData.Length; ++i)
            {
                if (srcData[i] != destData[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查二个文件的hash是否相同
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="destFile"></param>
        /// <returns>true 相同，false 不相同</returns>
        public bool CheckFileHash(string srcFile, string destFile)
        {
            //create md5 hasher
            MD5 md5Hasher = MD5.Create();

            //calc src file hash data
            byte[] srcData;
            using (FileStream fs = new FileStream(srcFile, FileMode.Open))
            {
                srcData = md5Hasher.ComputeHash(fs);
            }

            //calc dest file hash data
            byte[] destData;
            using (FileStream fs = new FileStream(destFile, FileMode.Open))
            {
                destData = md5Hasher.ComputeHash(fs);
            }

            //compare hash data size
            if (srcData.Length != destData.Length)
            {
                return false;
            }

            //compare hash data content
            for (int i = 0; i < srcData.Length; ++i)
            {
                if (srcData[i] != destData[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 取得文件的md5值
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public string GetFileMd5(string filepath)
        {
            byte[] data;
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                MD5 md5Hasher = MD5.Create();
                data = md5Hasher.ComputeHash(fs);
            }

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; ++i)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public string GetFileMd5(FileStream fileStream)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(fileStream);

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; ++i)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        string JoinPath(string a, string b)
        {
            if (string.IsNullOrEmpty(a))
            {
                return b;
            }

            if (string.IsNullOrEmpty(b))
            {
                return a;
            }
            return a + "/" + b;
        }

        public Manifest GetManifest()
        {
            Manifest manifest = new Manifest();

            Asset asset;
            foreach (string file in m_Modifies)
            {
                asset = new Asset();
                asset.path = file;
                asset.type = useDiffPatch ? Asset.AssetType.Patch : Asset.AssetType.Full;
                manifest.AddAsset(asset);
            }

            foreach (string file in m_Addes)
            {
                asset = new Asset();
                asset.path = file;
                asset.type = Asset.AssetType.Full;
                manifest.AddAsset(asset);
            }

            foreach (string file in m_Deletes)
            {
                asset = new Asset();
                asset.path = file;
                asset.type = Asset.AssetType.Delete;
                manifest.AddAsset(asset);
            }

            return manifest;
        }

        public void CollectAssets(Manifest manifest,string outDir)
        {
            FileInfo fileInfo;
            foreach (Asset asset in manifest.assets)
            {
                switch (asset.type)
                {
                    case Asset.AssetType.Full:
                        string filePath = Path.Combine(m_destPath, asset.path);
                        string outFilePath = Path.Combine(outDir, asset.path);
                        fileInfo = new FileInfo(filePath);
                        //保存文件大小
                        asset.size = fileInfo.Length;
                        //更新总的资源大小
                        manifest.totalSize += asset.size;
                        //保存md5
                        using (FileStream fs = fileInfo.OpenRead())
                        {
                            asset.hash = GetFileMd5(fs);
                        }

                       
                        if(!Directory.Exists(Path.GetDirectoryName(outFilePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(outFilePath));
                        }

                        //如果目标文件只读，则copy会失败.
                        FileInfo outFileInfo = new FileInfo(outFilePath);
                        if( (outFileInfo.Attributes & FileAttributes.ReadOnly)!=0)
                        {
                            outFileInfo.Attributes &= ~FileAttributes.ReadOnly;//outFileInfo.Attributes -= FileAttributes.ReadOnly;
                            outFileInfo.Refresh();
                        }

                        File.Copy(filePath, outFilePath, true);
                        FireProcessing(Segment.Collect, "collect " + filePath, 1);
                        break;
                    case Asset.AssetType.Patch:
                        //generate patch file
                        string srcfilePath = Path.Combine(m_srcPath, asset.path);
                        string destFilePath = Path.Combine(m_destPath, asset.path);
                        string patchFile = Path.Combine(outDir, asset.path);
                        if(!Directory.Exists(Path.GetDirectoryName(patchFile)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(patchFile));
                        }
                        using (FileStream output = new FileStream(patchFile, FileMode.Create))
                        {
                            BsDiff.BinaryPatchUtility.Create(File.ReadAllBytes(srcfilePath), File.ReadAllBytes(destFilePath), output);
                        }
                        break;
                }
            }
        }

        ///// <summary>
        ///// 生成manifest包含的资源
        ///// </summary>
        ///// <returns></returns>
        //public Manifest Collect()
        //{
        //    Manifest manifest = new Manifest();

        //    Asset asset;
        //    FileInfo fileInfo;
        //    string filePath;
        //    foreach (string file in m_Modifies)
        //    {
        //        asset = new Asset();
        //        asset.path = file;
        //        if(useDiffPatch)
        //        {
        //            asset.type = Asset.AssetType.Patch;
        //            //TODO 生成差异化文件
        //        }
        //        else
        //        {
        //            filePath = Path.Combine(m_destPath, file);
        //            //更新暂时使用文件替换
        //            asset.type =Asset.AssetType.Full;

        //            //更新的文件的信息从dest目录取。
        //            fileInfo = new FileInfo(filePath);
        //            //记录大小
        //            asset.size = fileInfo.Length;
        //            //记录md5
        //            using (FileStream fs = fileInfo.OpenRead())
        //            {
        //                asset.hash = GetFileMd5(fs);
        //            }

        //            File.Copy(filePath, Path.Combine(m_outPath, file));
        //        }

        //        manifest.AddAsset(asset);
        //    }

        //    foreach (string file in m_Addes)
        //    {
        //        asset = new Asset();
        //        asset.path = file;
        //        asset.type = Asset.AssetType.Full;

        //        filePath = Path.Combine(m_destPath, file);
        //        //新增的文件的信息从dest目录取。
        //        fileInfo = new FileInfo(filePath);
        //        //记录大小
        //        asset.size = fileInfo.Length;
        //        //记录md5
        //        using (FileStream fs = fileInfo.OpenRead())
        //        {
        //            asset.hash = GetFileMd5(fs);
        //        }

        //        File.Copy(filePath, Path.Combine(m_outPath, file));
        //        manifest.AddAsset(asset);
        //    }

        //    foreach (string file in m_Deletes)
        //    {
        //        asset = new Asset();
        //        asset.path = file;
        //        asset.type = Asset.AssetType.Delete;
        //        manifest.AddAsset(asset);
        //    }

        //    return manifest;
        //}

        private void FireProcessing(Segment segment, string msg, float percent)
        {
            if (OnProcessing != null)
                OnProcessing(segment, msg, percent);
        }

        public override string ToString()
        {
            StringBuilder sBuilder = new StringBuilder();
            foreach (string file in m_Addes)
            {
                sBuilder.Append("Add:" + file + "\n");
            }
            sBuilder.Append("---------------------------------\n");
            foreach (string file in m_Modifies)
            {
                sBuilder.Append("Mod:" + file + "\n");
            }
            sBuilder.Append("---------------------------------\n");
            foreach (string file in m_Deletes)
            {
                sBuilder.Append("Del:" + file + "\n");
            }
            return sBuilder.ToString();
        }
    }
}