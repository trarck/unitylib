using System;
using System.Collections.Generic;

namespace TK.Excel
{
    
    public class TypeInfo
    {
        public enum Sign
        {
            Object,//未知
            Byte,
            Int,
            Float,
            String,
            Boolean,
            Array,
            Dictionary,
            List,
            Long,
            Double,
            Generic
        }

        public string name { get; set; }
        public Sign sign { get; set; }
        public TypeInfo genericType { get; set; }

        public bool isGeneric { get { return sign == Sign.Generic; } }
        List<TypeInfo> _genericArguments;
        public List<TypeInfo> genericArguments
        {
            get
            {
                if (_genericArguments == null)
                {
                    _genericArguments = new List<TypeInfo>();
                }
                return _genericArguments;
            }
            set
            {
                _genericArguments = value;
            }
        }

        public bool isGenericArray { get { return isGeneric && genericArguments.Count == 1; } }
        public bool isGenericDictionary { get { return isGeneric && genericArguments.Count == 2; } }

        public TypeInfo(string name,Sign sign)
        {
            this.name = name;
            this.sign = sign;
        }

        public TypeInfo(string name, Sign sign, TypeInfo genericType)
            :this(name, sign)
        {
            this.genericType = genericType;
        }

        public TypeInfo(string name, Sign sign, TypeInfo genericType,List<TypeInfo> genericArguments)
             : this(name, sign, genericType)
        {
            _genericArguments = genericArguments;
        }

        public override string ToString()
        {
            if (!isGeneric)
            {
                return name;
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(genericType.name);
                sb.Append("<");
                for(int i=0,l= _genericArguments.Count; i < l; ++i)
                {
                    if (i != 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append(_genericArguments[i].ToString());
                }
                sb.Append(">");
                return sb.ToString();
            }
        }

        public Type ToSystemType()
        {
            Type t = null;

            switch (sign)
            {
                case Sign.Byte:
                    t = typeof(byte);
                    break;
                case Sign.Int:
                    t = typeof(int);
                    break;
                case Sign.Long:
                    t = typeof(long);
                    break;
                case Sign.Float:
                    t = typeof(float);
                    break;
                case Sign.Double:
                    t = typeof(double);
                    break;
                case Sign.String:
                    t = typeof(string);
                    break;
                case Sign.Boolean:
                    t = typeof(bool);
                    break;
                case Sign.Object:
                    t = typeof(object);
                    break;
                case Sign.Generic:
                    if (isGenericArray)
                    {
                        t = typeof(List<>);
                        Type[] typeArgs = { _genericArguments[0].ToSystemType() };
                        t = t.MakeGenericType(typeArgs);
                    }
                    else if (isGenericDictionary)
                    {
                        t = typeof(Dictionary<,>);
                        Type[] typeArgs = { _genericArguments[0].ToSystemType(), _genericArguments[1].ToSystemType() };
                        t = t.MakeGenericType(typeArgs);
                    }
                    break;
            }
            return t;
        }
        
        public static TypeInfo Byte = new TypeInfo("byte",Sign.Byte);
        public static TypeInfo Int = new TypeInfo("int",Sign.Int);
        public static TypeInfo Long = new TypeInfo("long",Sign.Long);
        public static TypeInfo Float = new TypeInfo("float",Sign.Float);
        public static TypeInfo Double = new TypeInfo("double",Sign.Double);
        public static TypeInfo String = new TypeInfo("string",Sign.String);
        public static TypeInfo Boolean = new TypeInfo("bool",Sign.Boolean);
        public static TypeInfo Object = new TypeInfo("object",Sign.Object);
        public static TypeInfo Array = new TypeInfo("Array",Sign.Array);
        public static TypeInfo List = new TypeInfo("List",Sign.List);
        public static TypeInfo Dictionary = new TypeInfo("Dictionary",Sign.Dictionary);
        public static Dictionary< int , Dictionary< int,
            string> > dd= new Dictionary<int, Dictionary
                <int, string>>();

        public static TypeInfo Parse(string type)
        {
            type = type.Trim();
            //primitive type
            switch (type)
            {
                case "byte":
                case "Byte":
                    return TypeInfo.Byte;
                case "int":
                case "Int":
                    return TypeInfo.Int;
                case "float":
                case "Float":
                    return TypeInfo.Float;
                case "string":
                case "String":
                    return TypeInfo.String;
                case "bool":
                case "Bool":
                    return TypeInfo.Boolean;
                case "long":
                case "Long":
                    return TypeInfo.Long;
                case "double":
                case "Double":
                    return TypeInfo.Double;
                case "array":
                case "Array":
                    return TypeInfo.Array;
                case "list":
                case "List":
                    return TypeInfo.List;
                case "dictionary":
                case "Dictionary":
                    return TypeInfo.Dictionary;
                case "object":
                case "Object":
                    return TypeInfo.Object;
            }

            //genetic type
            if (type.IndexOf("<") > -1)
            {
                return ParseGeneric(type);
            }
            //custom type
            return new TypeInfo(type,Sign.Object);
        }

        public class GenericStackInfo
        {
            public TypeInfo type;
            public int start;
        }

        public static TypeInfo ParseGeneric(string type)
        {
            char[] buff=new char[255];
            int len = 0;
            char c;
            Stack<GenericStackInfo> typeStack=new Stack<GenericStackInfo>();
            GenericStackInfo current =null;
            GenericStackInfo info = null;
            int start=0;
            for(int i = 0, l = type.Length; i < l; ++i)
            {
                c = type[i];
                
                switch (c)
                {
                    case '<':
                        info = new GenericStackInfo();
                        info.type = new TypeInfo(null,Sign.Generic, Parse(new string(buff, 0, len)));
                        len = 0;
                        info.start = start;
                        start = i+1;

                        if (current != null)
                        {
                            typeStack.Push(current);
                            current.type.genericArguments.Add(info.type);
                        }
                        current = info;
                        break;
                    case '>':
                        if (len > 0)
                        {
                            current.type.genericArguments.Add(Parse(new string(buff, 0, len)));
                            len = 0;
                        }

                        current.type.name = type.Substring(current.start, i+1-current.start);

                        if (typeStack.Count > 0)
                        {
                            current = typeStack.Pop();
                        }
                        break;
                    case ',':
                        if (len > 0)
                        {
                            current.type.genericArguments.Add(Parse(new string(buff, 0, len)));
                            len = 0;
                        }
                        start = i + 1;
                        break;
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        break;
                    default:
                        buff[len++] = c;
                        break;
                }
            }
            return current.type;
        }
    }
}