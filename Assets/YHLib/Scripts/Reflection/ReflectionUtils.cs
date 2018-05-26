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

        public static bool IsUnityPrimitiveType(Type type)
        {
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                return true;
            }
            else
            {
                switch (type.ToString())
                {
                    case "System.Sbyte":
                    case "System.Byte":
                    case "System.Int16":
                    case "System.Int32":
                    case "System.UInt32":
                    case "System.Int64":
                    case "System.UInt64":
                    case "System.Single":
                    case "System.Double":
                    case "System.Boolean":
                    case "System.String":

                    case "UnityEngine.Vect2":
                    case "UnityEngine.Vect3":
                    case "UnityEngine.Vect4":
                    case "UnityEngine.Rect":
                    case "UnityEngine.Bounds":
                    case "UnityEngine.Color":
                    case "UnityEngine.AnimationCurve":
                        return true;
                }
            }
            return false;
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
            if (memberInfo != null && obj!=null)
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
            if (memberInfo != null && obj!=null)
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

        public static object InvokeConstructor(Type type, object[] args)
        {
            if (type != null)
            {
                ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                for (int i = 0; i < constructors.Length; ++i)
                {

                    ParameterInfo[] parameters = constructors[i].GetParameters();
                    if (args != null && args.Length > 0)
                    {
                        if (parameters != null && args.Length == parameters.Length)
                        {
                            bool isMatch = true;

                            for (int j = 0; j < args.Length; ++j)
                            {
                                if (args[j] != null && parameters[j].ParameterType != typeof(System.Object) && parameters[j].ParameterType != args[j].GetType())
                                {
                                    isMatch = false;
                                }
                            }

                            if (isMatch)
                            {
                                return constructors[i].Invoke(args);
                            }
                        }
                    }
                    else if (parameters == null || parameters.Length == 0)
                    {
                        return constructors[i].Invoke(args);
                    }
                }
            }
            return null;
        }

        public static object InvokeMethod(object obj,string methodName,object[] args)
        {
            if (obj != null)
            {
                Type type = obj.GetType();
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public|BindingFlags.Static|BindingFlags.Instance|BindingFlags.NonPublic);
                for(int i = 0; i < methods.Length; ++i)
                {
                    if(methods[i].Name== methodName)
                    {
                        ParameterInfo[] parameters = methods[i].GetParameters();
                        if (args != null && args.Length>0)
                        {
                            if (parameters != null && args.Length==parameters.Length)
                            {
                                bool isMatch = true;

                                for(int j = 0; j < args.Length; ++j)
                                {
                                    if( args[j]!=null && parameters[j].ParameterType!=typeof(System.Object) && parameters[j].ParameterType != args[j].GetType())
                                    {
                                        isMatch = false;
                                    }
                                }

                                if (isMatch)
                                {
                                    return methods[i].Invoke(obj, args);
                                }
                            }
                        }
                        else if(parameters==null || parameters.Length==0)
                        {
                            return methods[i].Invoke(obj, args);
                        }
                    }
                }

            }
            return null;
        }

        #region Array
        public static int GetLength(object obj)
        {
            if (obj == null) return 0;
            Type type = obj.GetType();
            if (type.IsArray)
            {
                MethodInfo getLength = type.GetMethod("get_Length");
                return (int)getLength.Invoke(obj, null);
            }
            //TODO other type;
            return 0;
        }
        #endregion

    }
}