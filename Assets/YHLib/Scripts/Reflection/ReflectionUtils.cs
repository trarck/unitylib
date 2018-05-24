using System;
using System.Reflection;
using System.Collections.Generic;

namespace YH
{
    public class ReflectionUtils: Singleton<ReflectionUtils>
    {
        Dictionary<string,Assembly> m_Assemblies = new Dictionary<string, Assembly>();

        static BindingFlags AllFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public ReflectionUtils()
        {
            LoadBaseAssemblies();
        }

        void LoadBaseAssemblies()
        {
            //project runtimes
            LoadAssembly("Assembly-CSharp");

            //unity runtime
            LoadAssembly("UnityEngine");
        }

        public Assembly LoadAssembly(string name)
        {
            Assembly assembly = Assembly.Load(name);
            if (assembly != null)
            {
                m_Assemblies[name] = assembly;
            }

            Type[] types=assembly.GetTypes();
            return assembly;
        }

        public void LoadAssemblies(string[] names)
        {
            for(int i = 0; i < names.Length; ++i)
            {
                LoadAssembly(names[i]);
            }
        }

        public void RemoveAssembly(string name)
        {
            if (m_Assemblies.ContainsKey(name))
            {
                m_Assemblies.Remove(name);
            }
        }

        public void Clear()
        {
            m_Assemblies.Clear();
        }

        public Type GetType(string name)
        {
            Type type = null;
            foreach(var iter in m_Assemblies)
            {
                type=iter.Value.GetType(name);
                if (type != null)
                {
                    return type;
                }
            }

            return type;
        }


        public static FieldInfo[] GetFields(Type type,bool declaredOnly=true)
        {
            BindingFlags flags = AllFlags;
            if (declaredOnly)
            {
                flags |= BindingFlags.DeclaredOnly;
            }
            return type.GetFields(flags);
        }

        public static List<FieldInfo> GetSerializableFields(Type type, bool declaredOnly = true)
        {
            FieldInfo[] fields=GetFields(type,declaredOnly);
            List<FieldInfo> result = new List<FieldInfo>();

            FieldInfo fieldInfo = null;
            for (int i = 0; i < fields.Length; ++i)
            {
                fieldInfo = fields[i];

                if ((fieldInfo.Attributes == FieldAttributes.Private && fieldInfo.IsDefined
                    (typeof(UnityEngine.SerializeField), false))
                    || (fieldInfo.Attributes == FieldAttributes.Public && !fieldInfo.IsDefined(typeof(NonSerializedAttribute), false)))
                {
                    result.Add(fieldInfo);
                }
            }

            return result;
        }

        public static PropertyInfo[] GeProperties(Type type, bool declaredOnly = true)
        {
            BindingFlags flags = AllFlags;
            if (declaredOnly)
            {
                flags |= BindingFlags.DeclaredOnly;
            }
            return type.GetProperties(flags);
        }

        public static List<PropertyInfo> GeReadAndWriteableProperties(Type type, bool declaredOnly = true)
        {
            BindingFlags flags = AllFlags;
            if (declaredOnly)
            {
                flags |= BindingFlags.DeclaredOnly;
            }
            PropertyInfo[] properties= type.GetProperties(flags);
            List<PropertyInfo> result = new List<PropertyInfo>();
            for(int i = 0; i < properties.Length; ++i)
            {
                if(properties[i].CanRead && properties[i].CanWrite)
                {
                    result.Add(properties[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// get field and property
        /// </summary>
        /// <param name="type"></param>
        /// <param name="declaredOnly"></param>
        /// <returns></returns>
        public static List<MemberInfo> GetAccessableFieldAndProperties(Type type, bool declaredOnly = true)
        {
            List<MemberInfo> members = new List<MemberInfo>();
            members.AddRange(GetSerializableFields(type, declaredOnly).ToArray());
            members.AddRange(GeReadAndWriteableProperties(type, declaredOnly).ToArray());
            return members;
        }

        public static MemberInfo GetMember(Type type,string name)
        {
            MemberInfo[] members = type.GetMember(name, AllFlags);
            if (members != null)
            {
                return members[0];
            }
            return null;
        }


        public static void SetValue(MemberInfo memberInfo, object obj, object value)
        {
            if (memberInfo != null)
            {
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    (memberInfo as FieldInfo).SetValue(obj, value);
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    (memberInfo as PropertyInfo).SetValue(obj, value, null);
                }
            }
        }

        public static object GetValue(MemberInfo memberInfo, object obj)
        {
            if (memberInfo != null)
            {
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    return (memberInfo as FieldInfo).GetValue(obj);
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    return (memberInfo as PropertyInfo).GetValue(obj, null);
                }
            }
            return null;
        }

        public static Type GetFieldOrPropertyType(MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    return (memberInfo as FieldInfo).FieldType;
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    return (memberInfo as PropertyInfo).PropertyType;
                }
            }
            return null;
        }
    }
}