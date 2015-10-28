using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace YH
{
    /// <summary>
    /// 通用数据访问
    /// 特殊数据可以dao的子类来处理。
    /// </summary>
    public class DataTable
    {
        private static DataTable m_Instance;

        public static DataTable Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new DataTable();
                }
                return m_Instance;
            }
        }

        public static void DestroyInstance()
        {
            m_Instance = null;
        }

        DataDriver m_DataDriver;

        public DataTable()
        {
            m_DataDriver = new DataDriver("Assets/Resources/Data");
            m_DataDriver.dataFileExt = ".json";
        }

        public Dictionary<string, object> GetDictionaryData(string tableName,object key)
        {
            return Lookup<Dictionary<string,object>>(tableName,null,key);
        }

        public List<object> GetArrayData(string tableName,object key)
        {
            return Lookup<List<object>>(tableName, null, key);
        }

        public Dictionary<string, object> Lookup(string tableName, string columnName, params object[] args)
        {
            object data = m_DataDriver.FetchData(tableName);

            if (data != null)
            {
                return DataHelper.Fetch<Dictionary<string, object>>(data,columnName, args);
            }

            return null;
        }

        public T Lookup<T>(string tableName,string columnName,params object[] args)
        {
            object data = m_DataDriver.FetchData(tableName);
            
            if (data != null)
            {
                return DataHelper.Fetch<T>(data,columnName, args);
            }         

            return default(T);            
        }

        public DataDriver dataDriver
        {
            set
            {
                m_DataDriver = value;
            }

            get
            {
                return m_DataDriver;
            }
        }
    }
}