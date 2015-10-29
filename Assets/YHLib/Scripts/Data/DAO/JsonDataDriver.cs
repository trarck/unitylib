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
                object dataTable = fastJSON.JSON.Parse(content);
                return dataTable;
            }
            else
            {
                return null;
            }
        }

        public override void SaveDataToFile(string file, object data)
        {
            string content = fastJSON.JSON.ToJSON(data);
            File.WriteAllText(file, content);
        }
    }
}
