using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace YH
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
        
        public override object LoadDataFromFile(string file)
        {
            if (File.Exists(file))
            {
                string content = File.ReadAllText(file);
                object dataTable = null;// fastJSON.JSON.Parse(content);
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
