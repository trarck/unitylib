using UnityEngine;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace YH.Excel.Data
{
    public class EDDataHelper
    {

        public static List<string> GetHeader(ISheet sheet, int headerOffset = 0)
        {
            List<string> header = new List<string>();

            //first row is header as default
            IRow headerRow = sheet.GetRow(sheet.FirstRowNum + headerOffset);
            IEnumerator<ICell> iter = headerRow.GetEnumerator();
            while (iter.MoveNext())
            {
                header.Add(iter.Current.StringCellValue);
            }
            return header;
        }

        public static List<Field> PrepareHeaderFields(List<string> header, Schema schema)
        {
            List<Field> headerFields = new List<Field>();
            foreach (string name in header)
            {
                headerFields.Add(schema.GetField(name));
            }

            return headerFields;
        }

        public static object GetCellValue(ICell cell, ExcelDataType dataType)
        {
            switch (dataType)
            {
                case ExcelDataType.Int:
                    return EDDataHelper.GetIntValue(cell);
                case ExcelDataType.Float:
                    return EDDataHelper.GetFloatValue(cell);
                case ExcelDataType.Long:
                    return EDDataHelper.GetLongValue(cell);
                case ExcelDataType.Double:
                    return EDDataHelper.GetDoubleValue(cell);
                case ExcelDataType.Boolean:
                    return EDDataHelper.GetBoolValue(cell);
                case ExcelDataType.String:
                    return EDDataHelper.GetStringValue(cell);
                case ExcelDataType.Array:
                default:
                    break;
            }
            return null;
        }

        public static int GetIntValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return (int)cell.NumericCellValue;
                case CellType.String:
                    return int.Parse(cell.StringCellValue);
                case CellType.Boolean:
                    return cell.BooleanCellValue ? 1 : 0;
                default:
                    throw new System.Exception("can't convert to int from " + cell.CellType);
            }
        }

        public static long GetLongValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return (long)cell.NumericCellValue;
                case CellType.String:
                    return long.Parse(cell.StringCellValue);
                default:
                    throw new System.Exception("can't convert to long from " + cell.CellType);
            }
        }

        public static float GetFloatValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return (float)cell.NumericCellValue;
                case CellType.String:
                    return float.Parse(cell.StringCellValue);
                default:
                    throw new System.Exception("can't convert to float from " + cell.CellType);
            }
        }

        public static double GetDoubleValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.String:
                    return double.Parse(cell.StringCellValue);
                default:
                    throw new System.Exception("can't convert to double from " + cell.CellType);
            }
        }

        public static bool GetBoolValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return cell.NumericCellValue != 0;
                case CellType.String:
                    return bool.Parse(cell.StringCellValue);
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                default:
                    throw new System.Exception("can't convert to bool from " + cell.CellType);
            }
        }

        public static string GetStringValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                default:
                    return cell.StringCellValue;
            }
        }
    }
}