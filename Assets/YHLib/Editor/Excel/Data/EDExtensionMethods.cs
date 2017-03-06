using System;
using System.Collections.Generic;

namespace YH.Excel.Data
{
    public static class EDExtensionMethods
    {
        public static Type ToSystemType(this ExcelDataType dataType)
        {
            Type t = null;
            switch (dataType)
            {
                case ExcelDataType.Int:
                    t = typeof(int);
                    break;
                case ExcelDataType.Long:
                    t = typeof(long);
                    break;
                case ExcelDataType.Float:
                    t = typeof(float);
                    break;
                case ExcelDataType.Double:
                    t = typeof(double);
                    break;
                case ExcelDataType.String:
                    t = typeof(string);
                    break;
                case ExcelDataType.Boolean:
                    t = typeof(bool);
                    break;
                case ExcelDataType.Object:
                    t = typeof(object);
                    break;
            }
            return t;
        }

        public static Type ToSystemType(this Field field)
        {
            if (field.type == ExcelDataType.List)
            {
                return Type.GetType("System.Collections.Generic.List`1"+ "["+field.ExtTypeToSystemType().FullName+"]");
            }
            else if (field.type == ExcelDataType.Dictionary)
            {
                string memberType = field.extMemberType;
                string[] splits = memberType.Split(',');

                string fullMemberType = "";
                for (int i = 0; i < splits.Length; ++i)
                {
                    ExcelDataType dataType = EnumUtil.ParseEnum<ExcelDataType>(splits[i], ExcelDataType.String);
                    //不支持嵌套
                    fullMemberType += ((i == 0) ? "" : ",") + dataType.ToSystemType().FullName;
                }
                return Type.GetType("System.Collections.Generic.Dictionary`2" + "[" + fullMemberType + "]");
            }
            else
            {
                return field.type.ToSystemType();
            }
        }

        public static Type ExtTypeToSystemType(this Field field)
        {
            //现在还不支持复杂类型
            ExcelDataType dataType = EnumUtil.ParseEnum<ExcelDataType>(field.extType, ExcelDataType.String);
            return dataType.ToSystemType();
        }
    }
}
