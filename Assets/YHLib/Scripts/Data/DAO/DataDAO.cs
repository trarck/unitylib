using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace YH
{
    /// <summary>
    /// 任意类型数据访问
    /// </summary>
    class DataDAO:DAO
    {
        object m_Data;

        public DataDAO(object data)
        {
            m_Data = data;
        }

        public bool TryGetValue(object data, object keyOrIndex, out object value)
        {

            if (data is IList)
            {
                //获取列表
                if (keyOrIndex is int)
                {
                    value = (data as IList)[(int)keyOrIndex];

                    if (value != null)
                    {
                        return true;
                    }
                }
            }
            else if (data is IDictionary)
            {
                //获取字典
                IDictionary dict = data as IDictionary;
                if (dict.Contains(keyOrIndex))
                {
                    value = dict[keyOrIndex];
                    if (value != null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                //不支持的类型
                Debug.Log("Unkown Type");
            }

            value = null;
            return false;
        }

        public T Fetch<T>(object column, params object[] args)
        {
            object data = m_Data;

            object tempData = null;

            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (TryGetValue(data, args[i], out tempData))
                    {
                        data = tempData;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (TryGetValue(data, column, out tempData))
            {
                return (T)tempData;
            }

            //没找到，返回默认值
            return default(T);
        }

        public object data
        {
            set
            {
                m_Data = value;
            }

            get
            {
                return m_Data;
            }
        }
    }

    /// <summary>
    /// json数据访问
    /// json有二种复合数据，字典和数组。字典key是字符串，数组key是整型
    /// </summary>

    class JsonDataDao : DAO
    {
        object m_Data;

        public JsonDataDao(object data)
        {
            m_Data = data;
        }

        //获取一个值
        public T Fetch<T>(string columnName, params object[] args)
        {
            object data = m_Data;

            //判断是否有其它值
            if (args.Length > 0)
            {
                object tempData = null;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] is int)
                    {
                        tempData = (data as List<object>)[(int)args[i]];
                        if (tempData == null)
                        {
                            break;
                        }
                        else
                        {
                            data = tempData;
                        }
                    }
                    else if (args[i] is string)
                    {
                        if ((data as Dictionary<string, object>).TryGetValue(args[i] as string, out tempData))
                        {
                            data = tempData;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            //直接取值
            if (data is IDictionary)
            {
                Dictionary<string, object> dict = data as Dictionary<string, object>;

                if (dict != null)
                {
                    object value;
                    if (dict.TryGetValue(columnName, out value))
                    {
                        return (T)value;
                    }
                }
            }
            //出现错误，返回默认值
            return default(T);
        }

        public object data
        {
            set
            {
                m_Data = value;
            }

            get
            {
                return m_Data;
            }
        }
    }

    class DictionaryDataDao
    {
        Dictionary<string, object> m_Data;

        public T Fetch<T>(string columnName,string keyValue)
        {
            if (m_Data.ContainsKey(keyValue))
            {
                Dictionary<string, object> record = m_Data[keyValue] as Dictionary<string, object>;

                if (record.ContainsKey(columnName))
                {
                    return (T)record[columnName];
                }
            }
            return default(T);
        }

        public Dictionary<string, object> data
        {
            set
            {
                m_Data = value;
            }

            get
            {
                return m_Data;
            }
        }
    }

    class ListDictionaryDataDao
    {
        List<Dictionary<string, object>> m_Data;

        string m_PrimaryKey;

        public T Fetch<T>(string columnName,string keyValue )
        {
            foreach (Dictionary<string, object> record in m_Data)
            {
                if (record[m_PrimaryKey] == keyValue && record.ContainsKey(columnName))
                {
                    return (T)record[columnName]; 
                }
            }
            return default(T);
        }

        public T Fetch<T>(string columnName,int keyValue)
        {
            foreach (Dictionary<string, object> record in m_Data)
            {
                if ((int)record[m_PrimaryKey] == keyValue && record.ContainsKey(columnName))
                {
                    return (T)record[columnName]; 
                }
            }
            return default(T);
        }

        public List<Dictionary<string, object>> data
        {
            set
            {
                m_Data = value;
            }

            get
            {
                return m_Data;
            }
        }

        public string primaryKey
        {
            set
            {
                m_PrimaryKey = value;
            }

            get
            {
                return m_PrimaryKey;
            }
        }
    }
}