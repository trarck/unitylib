using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace YH.Excel.Data
{
    public static class TextExl {

        [MenuItem("Assets/Exl/Test")]
        public static void Test()
        {
            string fileName = Application.dataPath + "/Tests/Editor/Test.xlsx";

            IWorkbook workbook = ExcelHelper.Load(fileName);
            EDReader reader = new EDReader(workbook);

            ISheet dataSheet = workbook.GetSheetAt(0);

            IRow row = dataSheet.GetRow(0);

            for(int i= row.FirstCellNum;i<row.LastCellNum;++i)
            {
                ICell cell = row.GetCell(i);
                if (cell.Hyperlink!=null)
                {
                    Debug.LogFormat("Hype:{0},{1},{2},{3},{4},{5}", cell.Hyperlink.Label,cell.Hyperlink.Address,cell.Hyperlink.FirstRow,cell.Hyperlink.FirstColumn,cell.Hyperlink.LastRow,cell.Hyperlink.LastColumn);
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

            Schema schema = reader.ReadSchema(dataSheet);

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