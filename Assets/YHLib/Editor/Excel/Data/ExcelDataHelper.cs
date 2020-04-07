using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;

namespace TK.Excel
{
    public class ExcelDataHelper
    {
        public static T DeserializeObject<T>(Dictionary<string, object> data)
        {
            if (data == null)
            {
                return default(T);
            }

            Type type = typeof(T);
            object obj = YH.ReflectionUtils.InvokeConstructor(type, null);
            FieldInfo[] fields= YH.ReflectionUtils.GetFields(type);
            foreach(var fieldInfo in fields)
            {
                if (data.ContainsKey(fieldInfo.Name))
                {
                    fieldInfo.SetValue(obj, data[fieldInfo.Name]);
                }
            }
            return (T)obj;
        }

        public static List<T> DeserializeData<T>(IList data)
        {
            List<T> list = new List<T>();

            foreach (var it in data)
            {
                T obj = DeserializeObject<T>(it as Dictionary<string, object>);
                list.Add(obj);
            }

            return list;
        }

        public static Dictionary<string, object> SerializeObject(object obj)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            Type type = obj.GetType();
            FieldInfo[] fields = YH.ReflectionUtils.GetFields(type);
            foreach (var fieldInfo in fields)
            {
                data[fieldInfo.Name] = fieldInfo.GetValue(obj);
            }
            return data;
        }
    }
}