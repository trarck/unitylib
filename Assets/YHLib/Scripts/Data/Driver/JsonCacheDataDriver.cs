using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace YH.Data.Driver
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

        public  override object FetchData(string name)
        {
            if (m_Cache.ContainsKey(name))
            {
                return m_Cache[name];
            }
            string filePath = ParseDataFileName(name);
            object data = LoadDataFromFile(filePath);
            m_Cache[name] = data;
            return data;
        }

        public override List<T> FetchData<T>(string name)
        {
            if (m_Cache.ContainsKey(name))
            {
                return (List<T>)m_Cache[name];
            }
            string filePath = ParseDataFileName(name);
            List<T> data = LoadDataFromFile<List<T>>(filePath);
            m_Cache[name] = data;
            return data;
        }

        public override T FetchObject<T>(string name)
        {
            string filePath = ParseDataFileName(name);
            return LoadDataFromFile<T>(filePath);
        }

        public override void StoreData(object data, string name)
        {
            string filePath = ParseDataFileName(name);
            SaveDataToFile(filePath, data);
        }

        object LoadDataFromFile(string file)
        {
            if (File.Exists(file))
            {
                string content = File.ReadAllText(file);
                object dataTable = JsonUtility.FromJson<object>(content);
                return dataTable;
            }
			else
            {
                string ext = Path.GetExtension(file);
                TextAsset text = (TextAsset)Resources.Load(file.Replace(ext, ""));
                if (text)
                {
                    string content = text.text;
                    object dataTable = JsonUtility.FromJson<object>(content);
                    return dataTable;
                }
                return null;
            }
        }

        T LoadDataFromFile<T>(string file)
        {
            if (File.Exists(file))
            {
                string content = File.ReadAllText(file);
                T dataTable = JsonConvert.DeserializeObject<T>(content);
                return dataTable;
            }
            else
            {
                string ext = Path.GetExtension(file);
                TextAsset text = (TextAsset)Resources.Load(file.Replace(ext, ""));
                if (text)
                {
                    string content = text.text;
                    T dataTable = JsonConvert.DeserializeObject<T>(content);
                    return dataTable;
                }
                return default(T);
            }
        }

        void SaveDataToFile(string file, object data)
        {
            string content = JsonUtility.ToJson(data);
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
