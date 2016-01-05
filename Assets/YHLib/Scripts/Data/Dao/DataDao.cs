using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace YH
{

    /// <summary>
    /// 任意类型数据访问
    /// 目前只有查询功能
    /// </summary>
    public class DataDao : Dao
    {
        protected DataDriver m_DataDriver;

        protected string m_DataName;

        public DataDao()
        {

        }

        public DataDao(DataDriver dataDriver)
        {
            m_DataDriver = dataDriver;
        }

        public DataDao(DataDriver dataDriver,string dataName)
        {
            m_DataDriver = dataDriver;
            m_DataName = dataName;
        }

        public Dictionary<string, object> GetDictionaryData(object key)
        {
            return Lookup<Dictionary<string, object>>(null, key);
        }

        public List<object> GetArrayData( object key)
        {
            return Lookup<List<object>>(null, key);
        }

        public Dictionary<string, object> Lookup(string columnName, params object[] args)
        {
            object data = m_DataDriver.FetchData(m_DataName);

            if (data != null)
            {
                return DataHelper.Fetch<Dictionary<string, object>>(data, columnName, args);
            }

            return null;
        }

        public T Lookup<T>(string columnName, params object[] args)
        {
            object data = m_DataDriver.FetchData(m_DataName);

            if (data != null)
            {
                return DataHelper.Fetch<T>(data, columnName, args);
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

        public string dataName
        {
            set
            {
                m_DataName = value;
            }

            get
            {
                return m_DataName;
            }
        }
    }
}