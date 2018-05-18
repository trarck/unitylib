using System;
using System.Reflection;
using System.Collections.Generic;

namespace YH
{
    public class ReflectionUtils: Singleton<ReflectionUtils>
    {
        Dictionary<string,Assembly> m_Assemblies = new Dictionary<string, Assembly>();

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

        public FieldInfo[] GetFields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }


        public FieldInfo[] GetSerializableFields(Type type)
        {
            FieldInfo[] fields=type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<FieldInfo> result = new List<FieldInfo>();

            FieldInfo fieldInfo = null;
            for (int i = 0; i < fields.Length; ++i)
            {
                fieldInfo = fields[i];

                if ((fieldInfo.Attributes == FieldAttributes.Private && fieldInfo.IsDefined(typeof(UnityEngine.SerializeField), false))
                    || (fieldInfo.Attributes == FieldAttributes.Public && !fieldInfo.IsDefined(typeof(NonSerializedAttribute), false)))
                {
                    result.Add(fieldInfo);
                }
            }

            return result.ToArray();
        }
    }
}