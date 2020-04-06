using System.Collections.Generic;
using YH.Data.Driver;

namespace YH.Data.Dao
{

    /// <summary>
    /// 任意类型数据访问
    /// 目前只有查询功能
    /// </summary>
    public class DataDao : IDao
    {
        protected IDataDriver m_DataDriver;

        protected string m_DataName;

        public virtual bool Init(IDataDriver dataDriver)
        {
            m_DataDriver = dataDriver;
            return true;
        }

        public virtual bool Init(IDataDriver dataDriver,string dataName)
        {
            m_DataDriver = dataDriver;
            m_DataName = dataName;
            return true;
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

        public List<T> FetchListData<T>()
        {
            return FetchData<T>();
        }

        public List<T> FetchData<T>()
        {
            return m_DataDriver.FetchData<T>(m_DataName);
        }

        public Dictionary<K,T> FetchDictData<K,T>()
        {
            return m_DataDriver.FetchDict<K, T>(m_DataName);
        }

        public T FetchObject<T>()
        {
            return m_DataDriver.FetchObject<T>(m_DataName);
        }

        public IDataDriver dataDriver
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