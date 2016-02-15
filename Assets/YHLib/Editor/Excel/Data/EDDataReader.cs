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
                header = EDDataHelper.GetHeader(sheet);
            }

            List<Field> headerFields = EDDataHelper.PrepareHeaderFields(header, schema);

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
                header = EDDataHelper.GetHeader(sheet);
            }

            List<Field> headerFields = EDDataHelper.PrepareHeaderFields(header, schema);

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
            Dictionary<string, object> data = new Dictionary<string, object>();
            IEnumerator<ICell> iter = row.GetEnumerator();
            int index = 0;

            Field field;

            while (iter.MoveNext())
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
                    return EDLinkDataReader.GetLinkData<>
                default:
                    break;
            }
            return null;
        }
        
    }
}