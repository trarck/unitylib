using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace YH.Data.Dao
{
    /// <summary>
    /// 任意类型数据访问
    /// </summary>
    public class DataHelper
    {
        public static bool TryGetValue(object data, object keyOrIndex, out object value)
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

        public static T Fetch<T>(object data,object column, params object[] args)
        {
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

            if (column != null)
            {
                if (TryGetValue(data, column, out tempData))
                {
                    return (T)tempData;
                }
            }
            else
            {
                return (T)data;
            }

            //没找到，返回默认值
            return default(T);
        }
    }

    /// <summary>
    /// json数据访问
    /// json有二种复合数据，字典和数组。字典key是字符串，数组key是整型
    /// </summary>

    class JsonDataHelper
    {

        //获取一个值
        public static T Fetch<T>(object data,string columnName, params object[] args)
        {
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

            if (columnName != null)
            {
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
            }
            else
            {
                return (T)data;
            }

            //出现错误，返回默认值
            return default(T);
        }
    }

    class DictionaryDataHelper
    {
        public static T Fetch<T>(Dictionary<string, object> data,string columnName, string keyValue)
        {
            if (data.ContainsKey(keyValue))
            {
                Dictionary<string, object> record = data[keyValue] as Dictionary<string, object>;

                if (record.ContainsKey(columnName))
                {
                    return (T)record[columnName];
                }
            }
            return default(T);
        }
    }

    class ListDictionaryDataHelper
    {

        public static T Fetch<T>(List<Dictionary<string, object>> data, string primaryKey, string columnName, string keyValue)
        {
            foreach (Dictionary<string, object> record in data)
            {
                if ( (record[primaryKey] as string) == keyValue && record.ContainsKey(columnName))
                {
                    return (T)record[columnName];
                }
            }
            return default(T);
        }

        public T Fetch<T>(List<Dictionary<string, object>> data, string primaryKey, string columnName, int keyValue)
        {
            foreach (Dictionary<string, object> record in data)
            {
                if ((int)record[primaryKey] == keyValue && record.ContainsKey(columnName))
                {
                    return (T)record[columnName];
                }
            }
            return default(T);
        }
    }
}