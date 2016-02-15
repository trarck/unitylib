using UnityEngine;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using System;

namespace YH.Excel.Data
{
    public class EDLinkDataReader
    {
        public struct CellPosition
        {
            public int row;
            public int col;
        }

        public static CellPosition GetCellPosition(string posString)
        {
            CellPosition cp = new CellPosition();

            int row = 0;
            int col = 0;
            int i = 0;

            for (; i < posString.Length; ++i)
            {
                char c = posString[i];
                if ('a' <= c && c <= 'z')
                {
                    col = col * 26 + c - 'a' + 1;
                }
                else if ('A' <= c && c <= 'Z')
                {
                    col = col * 26 + c - 'A' + 1;
                }
                else
                {
                    break;
                }
            }

            for (; i < posString.Length; ++i)
            {
                char c = posString[i];
                if ('0' <= c && c <= '9')
                {
                    row = row * 10 + c - '0';
                }
                else
                {
                    throw new System.Exception("Parse link cell position errro for c=" + c);
                }
            }
            //索引号从0开始
            cp.row = row - 1;
            cp.col = col - 1;

            return cp;
        }

        static List<int> GetListInt(ISheet sheet, int rowIndex, int colIndex)
        {
            List<int> list = new List<int>();

            for (int i = sheet.FirstRowNum + rowIndex; i <= sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);
                ICell cell = row.GetCell(row.FirstCellNum + colIndex);
                list.Add(EDDataHelper.GetIntValue(cell));
            }
            return list;
        }

        static List<long> GetListLong(ISheet sheet, int rowIndex, int colIndex)
        {
            List<long> list = new List<long>();

            for (int i = sheet.FirstRowNum + rowIndex; i <= sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);
                ICell cell = row.GetCell(row.FirstCellNum + colIndex);
                list.Add(EDDataHelper.GetLongValue(cell));
            }
            return list;
        }

        static List<float> GetListFloat(ISheet sheet, int rowIndex, int colIndex)
        {
            List<float> list = new List<float>();

            for (int i = sheet.FirstRowNum + rowIndex; i <= sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);
                ICell cell = row.GetCell(row.FirstCellNum + colIndex);
                list.Add(EDDataHelper.GetFloatValue(cell));
            }
            return list;
        }

        static List<double> GetListDouble(ISheet sheet, int rowIndex, int colIndex)
        {
            List<double> list = new List<double>();

            for (int i = sheet.FirstRowNum + rowIndex; i <= sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);
                ICell cell = row.GetCell(row.FirstCellNum + colIndex);
                list.Add(EDDataHelper.GetDoubleValue(cell));
            }
            return list;
        }

        static List<string> GetListString(ISheet sheet, int rowIndex, int colIndex)
        {
            List<string> list = new List<string>();

            for (int i = sheet.FirstRowNum + rowIndex; i <= sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);
                ICell cell = row.GetCell(row.FirstCellNum + colIndex);
                list.Add(EDDataHelper.GetStringValue(cell));
            }
            return list;
        }

        static List<bool> GetListBool(ISheet sheet, int rowIndex, int colIndex)
        {
            List<bool> list = new List<bool>();

            for (int i = sheet.FirstRowNum + rowIndex; i <= sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);
                ICell cell = row.GetCell(row.FirstCellNum + colIndex);
                list.Add(EDDataHelper.GetBoolValue(cell));
            }
            return list;
        }

        static List<T> GetPrimitiveList<T>(ISheet sheet, int rowIndex, int colIndex, ExcelDataType dataType)
        {
            List<T> list = new List<T>();

            for (int i = sheet.FirstRowNum + rowIndex; i <= sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);
                ICell cell = row.GetCell(row.FirstCellNum + colIndex);
                list.Add((T)EDDataHelper.GetCellValue(cell, dataType));
            }
            return list;
        }

        static List<T> GetListData<T>(ISheet sheet, int rowIndex, int colIndex)
        {
            Type t = typeof(T);
            return GetListData(sheet, rowIndex, colIndex, t) as List<T>;
            //if (t == typeof(int) || t == typeof(int?))
            //{
            //    return GetListInt(sheet, rowIndex, colIndex) as List<T>;// GetPrimitiveList<T>(sheet, rowIndex, colIndex, ExcelDataType.Int);
            //}
            //else if (t == typeof(long) || t == typeof(long?))
            //{
            //    return GetPrimitiveList<T>(sheet, rowIndex, colIndex, ExcelDataType.Long);
            //}
            //else if (t == typeof(float) || t == typeof(float?))
            //{
            //    return GetPrimitiveList<T>(sheet, rowIndex, colIndex, ExcelDataType.Float);
            //}
            //else if (t == typeof(double) || t == typeof(double?))
            //{
            //    return GetPrimitiveList<T>(sheet, rowIndex, colIndex, ExcelDataType.Double);
            //}
            //else if (t == typeof(string))
            //{
            //    return GetListString(sheet, rowIndex, colIndex) as List<T>; //GetPrimitiveList<T>(sheet, rowIndex, colIndex, ExcelDataType.String);
            //}
            //else if (t == typeof(bool) || t == typeof(bool?))
            //{
            //    return GetPrimitiveList<T>(sheet, rowIndex, colIndex, ExcelDataType.Boolean);
            //}
            //else if (t == typeof(object))
            //{
            //    Schema schema = EDSchemaReader.ReadSchema(sheet);
            //    return EDDataReader.ReadList(sheet, schema) as List<T>;
            //}
            //else
            //{
            //    return null;
            //}
        }

        public static List<T> GetLinkData<T>(ICell cell)
        {
            string linkWhere = cell.StringCellValue;
            CellPosition cp;
            string linkSheetName = "";

            int pos = linkWhere.IndexOf("!");
            if (pos > -1)
            {
                //表的开始位置
                string linkCellPosition = linkWhere.Substring(pos + 1);
                cp = GetCellPosition(linkCellPosition);

                linkSheetName = linkWhere.Substring(0, pos);
            }
            else
            {
                //第一列，第一行
                cp = new CellPosition();
                cp.row = 0;
                cp.col = 0;

                linkSheetName = linkWhere;
            }

            ISheet linkSheet = cell.Sheet.Workbook.GetSheet(linkSheetName);

            return GetListData<T>(linkSheet, cp.row, cp.col);
        }


        static object GetListData(ISheet sheet, int rowIndex, int colIndex,Type t)
        {
            if (t == typeof(int) || t == typeof(int?))
            {
                return GetListInt(sheet, rowIndex, colIndex);
            }
            else if (t == typeof(long) || t == typeof(long?))
            {
                return GetListLong(sheet, rowIndex, colIndex);
            }
            else if (t == typeof(float) || t == typeof(float?))
            {
                return GetListFloat(sheet, rowIndex, colIndex);
            }
            else if (t == typeof(double) || t == typeof(double?))
            {
                return GetListDouble(sheet, rowIndex, colIndex);
            }
            else if (t == typeof(string))
            {
                return GetListString(sheet, rowIndex, colIndex);
            }
            else if (t == typeof(bool) || t == typeof(bool?))
            {
                return GetListFloat(sheet, rowIndex, colIndex);
            }
            else if (t == typeof(object))
            {
                Schema schema = EDSchemaReader.ReadSchema(sheet);
                return EDDataReader.ReadList(sheet, schema);
            }
            else
            {
                return null;
            }
        }

        public static object GetLinkData(ICell cell,Type t)
        {
            string linkWhere = cell.StringCellValue;
            CellPosition cp;
            string linkSheetName = "";

            int pos = linkWhere.IndexOf("!");
            if (pos > -1)
            {
                //表的开始位置
                string linkCellPosition = linkWhere.Substring(pos + 1);
                cp = GetCellPosition(linkCellPosition);

                linkSheetName = linkWhere.Substring(0, pos);
            }
            else
            {
                //第一列，第一行
                cp = new CellPosition();
                cp.row = 0;
                cp.col = 0;

                linkSheetName = linkWhere;
            }

            ISheet linkSheet = cell.Sheet.Workbook.GetSheet(linkSheetName);

            return GetListData(linkSheet, cp.row, cp.col,t);
        }
    }
}
