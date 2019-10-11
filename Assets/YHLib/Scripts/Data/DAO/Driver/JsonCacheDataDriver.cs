using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace YH
{
    /// <summary>
    /// Json格式数据读取
    /// 通常用于客户端的数据文件，所以带个缓存。
    /// </summary>
    public class JsonCacheDataDriver : DataDriver
    {
        Dictionary<string, object> m_Cache;

        public JsonCacheDataDriver()
        {
            m_Cache = new Dictionary<string, object>();
        }

        public JsonCacheDataDriver(string dataPath)
        {
            m_Cache = new Dictionary<string, object>();
            m_DataPath = dataPath;
            m_DataFileExt = ".json";
        }

        public override object FetchData(string name)
        {
            if (m_Cache.ContainsKey(name))
            {
                return m_Cache[name];
            }

            object data = LoadDataFromFile(m_DataPath + "/" + name + m_DataFileExt);
            m_Cache[name] = data;
            return data;
        }

        public override object LoadDataFromFile(string file)
        {
            if (File.Exists(file))
            {
                string content = File.ReadAllText(file);
                object dataTable = null;//fastJSON.JSON.Parse(content);
                return dataTable;
            }
			else
            {
                string ext = Path.GetExtension(file);
                TextAsset text = (TextAsset)Resources.Load(file.Replace(ext, ""));
                if (text)
                {
                    string content = text.text;
                    object dataTable = null;// fastJSON.JSON.Parse(content);
                    return dataTable;
                }
                return null;
            }
        }

        public override void SaveDataToFile(string file, object data)
        {
            string content = null;// fastJSON.JSON.ToJSON(data);
			//check Directory
            string dirPath = Path.GetDirectoryName(file);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllText(file, content);
        }
    }
}
