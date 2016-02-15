using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace YH.Excel.Data
{
    public static class TextExl
    {

        [MenuItem("Assets/Exl/Test")]
        public static void TestData()
        {
            string fileName = Application.dataPath + "/Tests/Editor/Test.xlsx";

            IWorkbook workbook = ExcelHelper.Load(fileName);
            ISheet sheet = workbook.GetSheetAt(0);
            Debug.Log(sheet.FirstRowNum + "," + sheet.LastRowNum);
            Test<object>();
        }

        static void Test<T>()
        {
            System.Type t = typeof(T);
            if (t == typeof(int) || t == typeof(int?))
            {
                Debug.Log("Int");
            }else
            if (t == typeof(long) || t == typeof(long?))
            {
                Debug.Log("Long");
            }else
            if (t == typeof(float) || t == typeof(float?))
            {
                Debug.Log("Float");
            }
            else
            {
                Debug.Log("other:"+t.Name+","+t.IsGenericType);
            }
        }

        public static void TestCell()
        {
            string fileName = Application.dataPath + "/Tests/Editor/Test.xlsx";

            IWorkbook workbook = ExcelHelper.Load(fileName);
            EDReader reader = new EDReader(workbook);

            ISheet dataSheet = workbook.GetSheetAt(0);

            IRow row = dataSheet.GetRow(0);

            for (int i = row.FirstCellNum; i < row.LastCellNum; ++i)
            {
                ICell cell = row.GetCell(i);
                if (cell.Hyperlink != null)
                {
                    Debug.LogFormat("Hype:{0},{1},{2},{3},{4},{5}", cell.Hyperlink.Label, cell.Hyperlink.Address, cell.Hyperlink.FirstRow, cell.Hyperlink.FirstColumn, cell.Hyperlink.LastRow, cell.Hyperlink.LastColumn);
                }

                Debug.LogFormat("Cell:{0},{1}", cell.CellType, cell.ColumnIndex);
            }
        }

        public static void TestCodeGen()
        {
            string fileName = Application.dataPath + "/Tests/Editor/Gacha.xlsx";

            IWorkbook workbook = ExcelHelper.Load(fileName);
            EDReader reader = new EDReader(workbook);

            ISheet dataSheet = workbook.GetSheetAt(0);

            Schema schema = EDSchemaReader.ReadSchema(dataSheet);

            foreach (Field field in schema.fields)
            {
                Debug.Log(field);
            }

            CodeGen gen = new CodeGen();

            gen.ns = "YH.Test.Data";

            string outputPath = Application.dataPath + "/Tests/Scripts/Data/Beans/";
            gen.GenClass(schema, outputPath);
        }
    }
}