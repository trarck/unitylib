﻿using System;

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
    }
}