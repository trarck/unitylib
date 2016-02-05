using UnityEngine;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace YH.Excel.Data
{
    public class EDDataReader
    {

        public EDDataReader()
        {

        }

        List<string> ReadHeader(ISheet sheet)
        {
            List<string> header = new List<string>();

            //first fow is header
            IRow headerRow = sheet.GetRow(sheet.FirstRowNum);
            IEnumerator<ICell> iter = headerRow.GetEnumerator();
            while (iter.MoveNext())
            {
                header.Add(iter.Current.StringCellValue);
            }
            return header;
        }

        List<Field> PrepareHeaderFields(List<string> header, Schema schema)
        {
            List<Field> headerFields = new List<Field>();
            foreach (string name in header)
            {
                headerFields.Add(schema.GetField(name));
            }

            return headerFields;
        }

        public Dictionary<string, object> ReadDictionary(ISheet sheet, Schema schema)
        {
            return ReadDictionary(sheet, schema, 0, null);
        }

        public Dictionary<string, object> ReadDictionary(ISheet sheet, Schema schema, int dataStartOffset)
        {
            return ReadDictionary(sheet, schema, dataStartOffset, null);
        }

        public Dictionary<string, object> ReadDictionary(ISheet sheet, Schema schema, int dataStartOffset, List<string> header)
        {

            if (header == null || header.Count == 0)
            {
                header = ReadHeader(sheet);
            }

            List<Field> headerFields = PrepareHeaderFields(header, schema);

            Dictionary<string, object> data = new Dictionary<string, object>();

            for (int i = sheet.FirstRowNum + dataStartOffset; i < sheet.LastRowNum; ++i)
            {

            }

            return data;

        }

        Dictionary<string, object> ReadRowData(IRow row, List<Field> headerFields)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            IEnumerator<ICell> iter = row.GetEnumerator();
            int index = 0;

            Field field;

            while (iter.MoveNext())
            {
                field = headerFields[index];

                switch (field.type)
                {
                    case ExcelDataType.Int:
                        data[field.name] = GetIntValue(iter.Current);
                        break;
                    case ExcelDataType.Float:
                        data[field.name] = GetFloatValue(iter.Current);
                        break;
                    case ExcelDataType.Long:
                        data[field.name] = GetLongValue(iter.Current);
                        break;
                    case ExcelDataType.Double:
                        data[field.name] = GetDoubleValue(iter.Current);
                        break;
                    case ExcelDataType.Boolean:
                        data[field.name] = GetBoolValue(iter.Current);
                        break;
                    case ExcelDataType.String:
                        data[field.name] = GetStringValue(iter.Current);
                        break;
                    case ExcelDataType.Array:
                    default:
                        break;
                }

                ++index;
            }

            return data;
        }

        int GetIntValue(ICell cell)
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

        long GetLongValue(ICell cell)
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

        float GetFloatValue(ICell cell)
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

        double GetDoubleValue(ICell cell)
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

        bool GetBoolValue(ICell cell)
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

        string GetStringValue(ICell cell)
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

        T[] GetArrayValue<T>(ICell cell)
        {
            List<T> list = new List<T>();

            string linkWhere = cell.StringCellValue;

            int pos = linkWhere.IndexOf("!");
            if (pos > -1)
            {
                string linkCell = linkWhere.Substring(pos + 1);
                string linkSheetName = linkWhere.Substring(0, pos);

                ISheet linkSheet = cell.Sheet.Workbook.GetSheet(linkSheetName);
            }

            return list.ToArray();
        }
    }
}