using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NPOI.SS.UserModel;

namespace TK.Excel
{
    public class DataWriter : IDataWriter
    {
        public HeadModel headModel { get; set; }

        private Side m_Side = Side.All;
        public Side side { get { return m_Side; } set { m_Side = value; } }
        
        #region row
        public void WriteRowDictionary(IRow row,  List<Field> headerFields, IDictionary data, int colStart = 0)
        {
            Field field;
            for (int i = 0, l = headerFields.Count; i < l; ++i)
            {
                field = headerFields[i];
                if (field.IsSameSide(side))
                {
                    int col = i + colStart;

                    ICell cell = row.GetCell(col);
                    if (cell == null)
                    {
                        cell = row.CreateCell(col);
                    }

                    if (data.Contains(field.name))
                    {
                        WriteHelper.SetCellValue(cell, data[field.name], field.type);
                    }
                }
            }
        }

        void WriteRowList(IRow row,List<Field> headerFields, IList data, int colStart = 0)
        {
            Field field;
            for (int i = 0, l = headerFields.Count; i < l; ++i)
            {
                field = headerFields[i];
                if (field.IsSameSide(side))
                {
                    int col = i + colStart;

                    ICell cell = row.GetCell(col);
                    if (cell == null)
                    {
                        cell = row.CreateCell(col);
                    }
                    WriteHelper.SetCellValue(cell, data[i], field.type);
                }
            }
        }
        static object GetValue(MemberInfo memberInfo, object obj)
        {
            if (memberInfo != null && obj != null)
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

        void WriteRowObject(IRow row,  List<Field> headerFields, object data, int colStart = 0)
        {
            Field field;
            Type dataType = data.GetType();

            for (int i = 0, l = headerFields.Count; i < l; ++i)
            {
                field = headerFields[i];
                if (field.IsSameSide(side))
                {
                    int col = i + colStart;

                    ICell cell = row.GetCell(col);
                    if (cell == null)
                    {
                        cell = row.CreateCell(col);
                    }

                    MemberInfo[] memberInfos= dataType.GetMember(field.name);
                    if(memberInfos!=null && memberInfos.Length > 0)
                    {
                        WriteHelper.SetCellValue(cell, GetValue(memberInfos[0],data), field.type);
                    }
                }
            }
        }

        void WriteRowData(IRow row, object data, List<Field> headerFields, int colStart=0)
        {
            if(data is IDictionary)
            {
                WriteRowDictionary(row,  headerFields, data as IDictionary, colStart);
            }
            else if (data is IList)
            {
                WriteRowList(row,headerFields, data as IList, colStart);
            }
            else
            {
                WriteRowObject(row, headerFields, data, colStart);
            }
        }
        #endregion

        #region list
        public void WriteList( ISheet sheet, Schema schema, IList list, HeadModel headModel)
        {
            WriteList(sheet, schema, list, headModel.DataRow, 0, null);
        }

        public void WriteList(ISheet sheet, Schema schema, IList list, int dataStart)
        {
            WriteList(sheet, schema, list, dataStart, 0, null);
        }

        public void WriteList( ISheet sheet, Schema schema, IList list, int dataStart, int colStart,List<string> header)
        {
            List<Field> headerFields = header == null ? schema.fields:ReadHelper.PrepareHeaderFields(header, schema);
            
            for (int i = 0,l=list.Count; i < l; ++i)
            {
                IRow row = sheet.GetRow(i + dataStart);
                if (row == null)
                {
                    row = sheet.CreateRow(i + dataStart);
                }
                WriteRowData(row, list[i],headerFields, colStart);
            }
        }

        #endregion

        #region dictionary

        public void WriteDictionary(ISheet sheet, Schema schema, IDictionary data, HeadModel headModel)
        {
            WriteDictionary(sheet, schema,data, headModel.DataRow, 0, null);
        }

        public void WriteDictionary(ISheet sheet, Schema schema, IDictionary data, int dataStart, int colStart, List<string> header)
        {

            List<Field> headerFields = header == null ? schema.fields : ReadHelper.PrepareHeaderFields(header, schema);
            int i = dataStart;
            foreach (var iter in data.Values)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                {
                    row = sheet.CreateRow(i);
                }
                WriteRowData(row, iter, headerFields, colStart);
                ++i;
            }
        }
        #endregion

        #region link
        //TODO link
        //List<T> ReadList<T>(ISheet sheet, int colIndex, int startRow, int endRow, TypeInfo dataType)
        //{
        //    List<T> list = new List<T>();
        //    int l = endRow <= 0 ? sheet.LastRowNum : (endRow < sheet.LastRowNum ? endRow : sheet.LastRowNum);
        //    for (int i = sheet.FirstRowNum + startRow; i <= l; ++i)
        //    {
        //        IRow row = sheet.GetRow(i);
        //        ICell cell = row.GetCell(row.FirstCellNum + colIndex);
        //        list.Add((T)GetCellValue(cell, dataType));
        //    }
        //    return list;
        //}

        //object ReadListData(ISheet sheet, int colIndex, int startRow, int endRow, TypeInfo t)
        //{
        //    switch (t.sign)
        //    {
        //        case TypeInfo.Sign.Int:
        //            return ReadList<int>(sheet, colIndex, startRow, endRow, t);
        //        case TypeInfo.Sign.Float:
        //            return ReadList<float>(sheet, colIndex, startRow, endRow, t);
        //        case TypeInfo.Sign.Long:
        //            return ReadList<long>(sheet, colIndex, startRow, endRow, t);
        //        case TypeInfo.Sign.Double:
        //            return ReadList<double>(sheet, colIndex, startRow, endRow, t);
        //        case TypeInfo.Sign.Boolean:
        //            return ReadList<bool>(sheet, colIndex, startRow, endRow, t);
        //        case TypeInfo.Sign.String:
        //            return ReadList<string>(sheet, colIndex, startRow, endRow, t);
        //        default:
        //            Schema schema = SchemaReader.ReadSchema(sheet,headModel);
        //            return ReadList(sheet, schema, startRow, endRow);
        //    }
        //}

        //public object ReadLinkList(ICell cell, TypeInfo t)
        //{
        //    if (cell == null || cell.StringCellValue == "") return null;

        //    string linkWhere = cell.StringCellValue;
        //    CellPosition cp;
        //    string linkSheetName =ReadLinkHelper.ParseLinkCell(cell, out cp);

        //    ISheet linkSheet = cell.Sheet.Workbook.GetSheet(linkSheetName);

        //    return ReadListData(linkSheet, cp.colStart, cp.rowStart, cp.rowEnd, t);
        //}

        //public object ReadLinkArray(ICell cell, TypeInfo t)
        //{
        //    return ReadLinkList(cell, t); ;
        //}

        //public  object ReadLinkDict(ICell cell, string keyField = null, bool removeKeyFieldInElement = false)
        //{
        //    if (cell == null || cell.StringCellValue == "") return null;
        //    string linkWhere = cell.StringCellValue;
        //    CellPosition cp;
        //    string cellKey;
        //    string linkSheetName = ReadLinkHelper.ParseLinkCell(cell, out cp, out cellKey);
        //    if (string.IsNullOrEmpty(keyField))
        //    {
        //        keyField = cellKey;
        //    }

        //    ISheet linkSheet = cell.Sheet.Workbook.GetSheet(linkSheetName);
        //    Schema schema = SchemaReader.ReadSchema(linkSheet,headModel);

        //    //内容要跳过头
        //    return ReadDictionary(linkSheet, schema, keyField, cp.rowStart, cp.colStart, cp.colEnd + 1, null, removeKeyFieldInElement, cp.rowEnd);
        //}

        //public object ReadLinkObject(ICell cell, TypeInfo t)
        //{
        //    if (cell == null || cell.StringCellValue == "") return null;
        //    string linkWhere = cell.StringCellValue;
        //    CellPosition cp;
        //    string linkSheetName = ReadLinkHelper.ParseLinkCell(cell, out cp);
        //    if (cp.rowEnd <= 0)
        //    {
        //        cp.rowEnd = cp.rowStart;
        //    }

        //    ISheet linkSheet = cell.Sheet.Workbook.GetSheet(linkSheetName);
        //    Schema schema = SchemaReader.ReadSchema(linkSheet,headModel);

        //    //内容要跳过头
        //    return ReadList(linkSheet, schema, cp.rowStart, cp.rowEnd, cp.colStart, cp.colEnd + 1, null)[0];
        //}
        #endregion
    }
}