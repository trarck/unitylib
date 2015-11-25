using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace YH
{
    public class DataDriver
    {
        //数据存放的路径
        protected string m_DataPath;

        //数据文件的扩展名
        protected string m_DataFileExt = ".dat";

        public DataDriver()
        {

        }

        public DataDriver(string dataPath)
        {
            m_DataPath = dataPath;
        }

        public virtual object FetchData(string name)
        {
            return LoadDataFromFile(m_DataPath + "/" + name+m_DataFileExt);
        }

        public virtual void StoreData(string name, object data)
        {
            SaveDataToFile(m_DataPath + "/" + name + m_DataFileExt, data);
        }

        public virtual object LoadDataFromFile(string file)
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
                    object dataTable = null;//fastJSON.JSON.Parse(content);
                    return dataTable;
                }
                return null;
            }
        }

        public virtual void SaveDataToFile(string file, object data)
        {
            string content = "";// fastJSON.JSON.ToJSON(data);
            File.WriteAllText(file, content);
        }

        public string dataPath
        {
            set
            {
                m_DataPath = value;
            }

            get
            {
                return m_DataPath;
            }
        }

        public string dataFileExt
        {
            set
            {
                m_DataFileExt = value;
            }

            get
            {
                return m_DataFileExt;
            }
        }
    }
}
