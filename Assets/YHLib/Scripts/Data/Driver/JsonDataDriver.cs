using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace YH.Data.Driver
{
    /// <summary>
    /// Json格式数据读取
    /// </summary>
    public class JsonDataDriver : DataDriver
    {

        public JsonDataDriver()
        {
        }

        public JsonDataDriver(string dataPath)
        {
            m_DataPath = dataPath;
            m_DataFileExt = ".json";
        }

        public override object FetchData(string name)
        {
            string filePath = ParseDataFileName(name);
            return LoadDataFromFile(filePath); ;
        }

        public override List<T> FetchData<T>(string name)
        {
            string filePath = ParseDataFileName(name);
            return LoadDataFromFile<List<T>>(filePath);
        }

        public override T FetchObject<T>(string name)
        {
            string filePath = ParseDataFileName(name);
            return LoadDataFromFile<T>(filePath);
        }

        public override void StoreData(object data,string name)
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
                T dataTable = JsonUtility.FromJson<T>(content);
                return dataTable;
            }
            else
            {
                string ext = Path.GetExtension(file);
                TextAsset text = (TextAsset)Resources.Load(file.Replace(ext, ""));
                if (text)
                {
                    string content = text.text;
                    T dataTable = JsonUtility.FromJson<T>(content);
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
