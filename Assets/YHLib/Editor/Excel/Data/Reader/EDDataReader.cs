using System.Collections;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace YH.Excel.Data
{
    public class EDDataReader
    {

        public EDDataReader()
        {

        }

        public static List<object> ReadList(ISheet sheet, Schema schema)
        {
            return ReadList(sheet, schema, 2, null);
        }

        public static List<object> ReadList(ISheet sheet, Schema schema, int dataStartOffset)
        {
            return ReadList(sheet, schema, dataStartOffset, null);
        }

        public static List<object> ReadList(ISheet sheet, Schema schema, int dataStartOffset, List<string> header)
        {

            if (header == null || header.Count == 0)
            {
                header = EDReadHelper.GetHeader(sheet);
            }

            List<Field> headerFields = EDReadHelper.PrepareHeaderFields(header, schema);

            List<object> list = new List<object>();

            for (int i = sheet.FirstRowNum + dataStartOffset; i <= sheet.LastRowNum; ++i)
            {
                Dictionary<string, object> record = ReadRowData(sheet.GetRow(i), headerFields);
                list.Add(record);
            }
            return list;
        }

        public static Dictionary<string, object> ReadDictionary(ISheet sheet, Schema schema)
        {
            return ReadDictionary(sheet, schema, "", 2, null);
        }

        public static Dictionary<string, object> ReadDictionary(ISheet sheet, Schema schema, string keyField)
        {
            return ReadDictionary(sheet, schema, keyField, 2, null);
        }

        public static Dictionary<string, object> ReadDictionary(ISheet sheet, Schema schema, string keyField, int dataStartOffset, List<string> header)
        {

            if (header == null || header.Count == 0)
            {
                header = EDReadHelper.GetHeader(sheet);
            }

            List<Field> headerFields = EDReadHelper.PrepareHeaderFields(header, schema);

            //如果没指定key,则默认使用第一个
            if (string.IsNullOrEmpty(keyField))
            {
                keyField = header[0];
            }

            Dictionary<string, object> dict = new Dictionary<string, object>();

            for (int i = sheet.FirstRowNum + dataStartOffset; i <= sheet.LastRowNum; ++i)
            {
                Dictionary<string, object> record = ReadRowData(sheet.GetRow(i), headerFields);
                string key = record[keyField].ToString();
                dict[key] = record;
            }
            return dict;
        }

        static Dictionary<string, object> ReadRowData(IRow row, List<Field> headerFields)
        {
            if (headerFields == null || headerFields.Count == 0) return null;

            Dictionary<string, object> data = new Dictionary<string, object>();
            IEnumerator<ICell> iter = row.GetEnumerator();
            int index = 0;

            Field field;

            while (iter.MoveNext() && index<headerFields.Count)
            {
                field = headerFields[index];
                data[field.name] = GetCellValue(iter.Current, field);
                ++index;
            }

            return data;
        }

        public static object GetCellValue(ICell cell, Field field)
        {
            switch (field.type)
            {
                case ExcelDataType.Int:
                    return EDReadHelper.GetIntValue(cell);
                case ExcelDataType.Float:
                    return EDReadHelper.GetFloatValue(cell);
                case ExcelDataType.Long:
                    return EDReadHelper.GetLongValue(cell);
                case ExcelDataType.Double:
                    return EDReadHelper.GetDoubleValue(cell);
                case ExcelDataType.Boolean:
                    return EDReadHelper.GetBoolValue(cell);
                case ExcelDataType.String:
                    return EDReadHelper.GetStringValue(cell);
                case ExcelDataType.List:
                    return EDLinkDataReader.GetLinkData(cell, field.ExtTypeToSystemType());
                case ExcelDataType.Array:
                    return EDLinkDataReader.GetLinkArray(cell, field.ExtTypeToSystemType());
                case ExcelDataType.Dictionary:
                    return EDLinkDataReader.GetLinkDict(cell, field.extTypeKeyField);
                default:
                    break;
            }
            return null;
        }
    }
}