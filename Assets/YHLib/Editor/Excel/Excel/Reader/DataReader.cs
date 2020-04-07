using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace TK.Excel
{
    public class DataReader : IDataReader
    {
        public HeadModel headModel { get; set; }
        public Side side { get; set; }

        #region cell
        public object GetCellValue(ICell cell, TypeInfo dataType)
        {
            switch (dataType.sign)
            {
                case TypeInfo.Sign.Byte:
                    return ReadHelper.GetIntValue(cell);
                case TypeInfo.Sign.Int:
                    return ReadHelper.GetIntValue(cell);
                case TypeInfo.Sign.Float:
                    return ReadHelper.GetFloatValue(cell);
                case TypeInfo.Sign.Long:
                    return ReadHelper.GetLongValue(cell);
                case TypeInfo.Sign.Double:
                    return ReadHelper.GetDoubleValue(cell);
                case TypeInfo.Sign.Boolean:
                    return ReadHelper.GetBoolValue(cell);
                case TypeInfo.Sign.String:
                    return ReadHelper.GetStringValue(cell);
                case TypeInfo.Sign.Array:
                case TypeInfo.Sign.List:
                case TypeInfo.Sign.Dictionary:
                    return GetCompositeValue(cell, dataType);
                case TypeInfo.Sign.Generic:
                    return GetGenericValue(cell, dataType);
                case TypeInfo.Sign.Object:
                    return GetObjectValue(cell, dataType);
                default:
                    break;
            }
            return null;
        }
        
        public object GetCompositeValue(ICell cell, TypeInfo type)
        {
            if (ReadHelper.IsLinkCell(cell))
            {
                switch (type.sign)
                {
                    case TypeInfo.Sign.Array:
                        return ReadLinkArray(cell, TypeInfo.Object);
                    case TypeInfo.Sign.List:
                        return ReadLinkList(cell, TypeInfo.Object);
                    case TypeInfo.Sign.Dictionary:
                        return ReadLinkDict(cell, null);
                    default:
                        return null;
                }
            }
            else
            {
                //as json data
                return Newtonsoft.Json.JsonConvert.DeserializeObject(cell.StringCellValue, type.ToSystemType());
            }
        }

        public object GetGenericValue(ICell cell, TypeInfo type)
        {
            if (ReadHelper.IsLinkCell(cell))
            {
                switch (type.genericType.sign)
                {
                    case TypeInfo.Sign.Array:
                        return ReadLinkArray(cell, type.genericArguments[0]);
                    case TypeInfo.Sign.List:
                        return ReadLinkList(cell, type.genericArguments[0]);
                    case TypeInfo.Sign.Dictionary:
                        return ReadLinkDict(cell, null);
                    default:
                        return null;
                }
            }
            else
            {
                if (cell != null)
                {
                    //as json data
                    return Newtonsoft.Json.JsonConvert.DeserializeObject(cell.StringCellValue,type.ToSystemType());
                }
                else
                {
                    return null;
                }
            }
        }

        public object GetObjectValue(ICell cell, TypeInfo type)
        {
            if (ReadHelper.IsLinkCell(cell))
            {
                return ReadLinkObject(cell, type);
            }
            else
            {
                //as json data
                return Newtonsoft.Json.JsonConvert.DeserializeObject(cell.StringCellValue, type.ToSystemType());
            }
        }
        #endregion

        #region row

        public Dictionary<string, object> ReadRowData(IRow row, List<Field> headerFields)
        {
            return ReadRowData(row,headerFields,0);
        }

        Dictionary<string, object> ReadRowData(IRow row, List<Field> headerFields, int colStart, int colEnd = -1)
        {
            if (headerFields == null || headerFields.Count == 0) return null;

            Dictionary<string, object> data = new Dictionary<string, object>();
            int index = 0;

            Field field;
            int l = colEnd < 1 ? row.LastCellNum : (colEnd < row.LastCellNum ? colEnd : row.LastCellNum);
            //offset 相对于0开始，excel最左边一列不能为空
            for (int i = row.FirstCellNum + colStart; i < l; ++i)
            {
                field = headerFields[index];
                if (field.IsSameSide(side))
                {
                    data[field.name] = GetCellValue(row.GetCell(i), field.type);
                }
                ++index;
            }

            return data;
        }
        #endregion

        #region list
        public List<object> ReadList(ISheet sheet, Schema schema, HeadModel headModel)
        {
            return ReadList(sheet, schema, headModel.DataRow, -1, 0, -1, null);
        }

        public List<object> ReadList(ISheet sheet, Schema schema, int dataStart)
        {
            return ReadList(sheet, schema, dataStart, -1, 0, -1, null);
        }

        public List<object> ReadList(ISheet sheet, Schema schema, int dataStart, int dataEnd)
        {
            return ReadList(sheet, schema, dataStart, dataEnd, 0, -1, null);
        }

        public List<object> ReadList(ISheet sheet, Schema schema, int dataStart, int dataEnd, int colStart, int colEnd, List<string> header)
        {

            if (header == null || header.Count == 0)
            {
                header = ReadHelper.GetHeader(sheet, 0, colStart, colEnd);
            }

            List<Field> headerFields = ReadHelper.PrepareHeaderFields(header, schema);

            List<object> list = new List<object>();
            int l = dataEnd <= 0 ? sheet.LastRowNum : (dataEnd < sheet.LastRowNum ? dataEnd : sheet.LastRowNum);
            for (int i = sheet.FirstRowNum + dataStart; i <= l; ++i)
            {
                Dictionary<string, object> record = ReadRowData(sheet.GetRow(i), headerFields, colStart, colEnd);
                list.Add(record);
            }
            return list;
        }

        #endregion

        #region dictionary

        public IDictionary ReadDictionary(ISheet sheet, Schema schema, HeadModel headModel)
        {
            return ReadDictionary(sheet, schema, "", headModel.DataRow, 0, -1, null);
        }

        public IDictionary ReadDictionary(ISheet sheet, Schema schema, string keyField, HeadModel headModel)
        {
            return ReadDictionary(sheet, schema, keyField, headModel.DataRow, 0, -1, null);
        }

        public IDictionary ReadDictionary(ISheet sheet, Schema schema, string keyField, int dataStart, int colStart, int colEnd, List<string> header, bool removeKeyInElement = false, int dataEnd = -1)
        {

            if (header == null || header.Count == 0)
            {
                header = ReadHelper.GetHeader(sheet, 0, colStart, colEnd);
            }

            List<Field> headerFields = ReadHelper.PrepareHeaderFields(header, schema);

            //如果没指定key,则默认使用第一个
            if (string.IsNullOrEmpty(keyField))
            {
                keyField = header[0];
            }

            Dictionary<object, object> dict = new Dictionary<object, object>();
            int l = dataEnd <= 0 ? sheet.LastRowNum : (dataEnd < sheet.LastRowNum ? dataEnd : sheet.LastRowNum);
            for (int i = sheet.FirstRowNum + dataStart; i <= l; ++i)
            {
                Dictionary<string, object> record = ReadRowData(sheet.GetRow(i), headerFields, colStart, colEnd);
                object key = record[keyField];
                dict[key] = record;
                if (removeKeyInElement)
                {
                    record.Remove(keyField);
                }
            }
            return dict;
        }
        #endregion

        #region link
        List<T> ReadList<T>(ISheet sheet, int colIndex, int startRow, int endRow, TypeInfo dataType)
        {
            List<T> list = new List<T>();
            int l = endRow <= 0 ? sheet.LastRowNum : (endRow < sheet.LastRowNum ? endRow : sheet.LastRowNum);
            for (int i = sheet.FirstRowNum + startRow; i <= l; ++i)
            {
                IRow row = sheet.GetRow(i);
                ICell cell = row.GetCell(row.FirstCellNum + colIndex);
                list.Add((T)GetCellValue(cell, dataType));
            }
            return list;
        }

        object ReadListData(ISheet sheet, int colIndex, int startRow, int endRow, TypeInfo t)
        {
            switch (t.sign)
            {
                case TypeInfo.Sign.Byte:
                    return ReadList<byte>(sheet, colIndex, startRow, endRow, t);
                case TypeInfo.Sign.Int:
                    return ReadList<int>(sheet, colIndex, startRow, endRow, t);
                case TypeInfo.Sign.Float:
                    return ReadList<float>(sheet, colIndex, startRow, endRow, t);
                case TypeInfo.Sign.Long:
                    return ReadList<long>(sheet, colIndex, startRow, endRow, t);
                case TypeInfo.Sign.Double:
                    return ReadList<double>(sheet, colIndex, startRow, endRow, t);
                case TypeInfo.Sign.Boolean:
                    return ReadList<bool>(sheet, colIndex, startRow, endRow, t);
                case TypeInfo.Sign.String:
                    return ReadList<string>(sheet, colIndex, startRow, endRow, t);
                default:
                    Schema schema = SchemaReader.ReadSchema(sheet,headModel);
                    return ReadList(sheet, schema, startRow, endRow);
            }
        }

        public object ReadLinkList(ICell cell, TypeInfo t)
        {
            if (cell == null || cell.StringCellValue == "") return null;

            string linkWhere = cell.StringCellValue;
            CellPosition cp;
            string linkSheetName =ReadLinkHelper.ParseLinkCell(cell, out cp);

            ISheet linkSheet = cell.Sheet.Workbook.GetSheet(linkSheetName);

            return ReadListData(linkSheet, cp.colStart, cp.rowStart, cp.rowEnd, t);
        }

        public object ReadLinkArray(ICell cell, TypeInfo t)
        {
            return ReadLinkList(cell, t); ;
        }

        public  object ReadLinkDict(ICell cell, string keyField = null, bool removeKeyFieldInElement = false)
        {
            if (cell == null || cell.StringCellValue == "") return null;
            string linkWhere = cell.StringCellValue;
            CellPosition cp;
            string cellKey;
            string linkSheetName = ReadLinkHelper.ParseLinkCell(cell, out cp, out cellKey);
            if (string.IsNullOrEmpty(keyField))
            {
                keyField = cellKey;
            }

            ISheet linkSheet = cell.Sheet.Workbook.GetSheet(linkSheetName);
            Schema schema = SchemaReader.ReadSchema(linkSheet,headModel);

            //内容要跳过头
            return ReadDictionary(linkSheet, schema, keyField, cp.rowStart, cp.colStart, cp.colEnd + 1, null, removeKeyFieldInElement, cp.rowEnd);
        }

        public object ReadLinkObject(ICell cell, TypeInfo t)
        {
            if (cell == null || cell.StringCellValue == "") return null;
            string linkWhere = cell.StringCellValue;
            CellPosition cp;
            string linkSheetName = ReadLinkHelper.ParseLinkCell(cell, out cp);
            if (cp.rowEnd <= 0)
            {
                cp.rowEnd = cp.rowStart;
            }

            ISheet linkSheet = cell.Sheet.Workbook.GetSheet(linkSheetName);
            Schema schema = SchemaReader.ReadSchema(linkSheet,headModel);

            //内容要跳过头
            return ReadList(linkSheet, schema, cp.rowStart, cp.rowEnd, cp.colStart, cp.colEnd + 1, null)[0];
        }
        #endregion
    }
}