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
            string fileName = Application.dataPath + "/Tests/Editor/Gacha.xlsx";

            IWorkbook workbook = ExcelHelper.Load(fileName);
            ISheet sheet = workbook.GetSheetAt(0);
            Schema schema = EDSchemaReader.ReadSchema(sheet);
            EDDataReader reader = new EDDataReader();

            List<object> dataList= EDDataReader.ReadList(sheet, schema);
            Debug.Log(dataList.Count);

            foreach(Dictionary<string, object> iter in dataList)
            {
                Debug.LogFormat("{0},{1},{2}", iter["name"], iter["probability"], iter["items"]);
                string[] items = iter["items"] as string[];
                string o = "";
                foreach(string s in items)
                {
                    o += s + ",";
                }

                Debug.Log(o);
            }
        }

        [MenuItem("Assets/Exl/Test1")]
        public static void TestData1()
        {
            string fileName = Application.dataPath + "/Tests/Editor/Gacha1.xlsx";

            IWorkbook workbook = ExcelHelper.Load(fileName);
            ISheet sheet = workbook.GetSheetAt(0);
            Schema schema = EDSchemaReader.ReadSchema(sheet);
            EDDataReader reader = new EDDataReader();

            List<object> dataList = EDDataReader.ReadList(sheet, schema);
            Debug.Log(dataList.Count);

            foreach (Dictionary<string, object> iter in dataList)
            {
                Debug.LogFormat("{0},{1},{2}", iter["name"], iter["probability"], iter["items"]);
                List<int> items = iter["items"] as List<int>;
                string o = "";
                foreach (int s in items)
                {
                    o += s + ",";
                }

                Debug.Log(o);
            }
        }

        [MenuItem("Assets/Exl/Test2")]
        public static void TestData2()
        {
            string fileName = Application.dataPath + "/Tests/Editor/Gacha2.xlsx";

            IWorkbook workbook = ExcelHelper.Load(fileName);
            ISheet sheet = workbook.GetSheetAt(0);
            Schema schema = EDSchemaReader.ReadSchema(sheet);
            EDDataReader reader = new EDDataReader();

            List<object> dataList = EDDataReader.ReadList(sheet, schema);
            Debug.Log(dataList.Count);

            foreach (Dictionary<string, object> iter in dataList)
            {
                Debug.LogFormat("{0},{1},{2}", iter["name"], iter["probability"], iter["items"]);
                List<bool> items = iter["items"] as List<bool>;
                string o = "";
                foreach (bool s in items)
                {
                    o += s + ",";
                }

                Debug.Log(o);
            }
        }

        [MenuItem("Assets/Exl/Test3")]
        public static void TestData3()
        {
            string fileName = Application.dataPath + "/Tests/Editor/Gacha3.xlsx";

            IWorkbook workbook = ExcelHelper.Load(fileName);
            ISheet sheet = workbook.GetSheetAt(0);
            Schema schema = EDSchemaReader.ReadSchema(sheet);
            EDDataReader reader = new EDDataReader();

            List<object> dataList = EDDataReader.ReadList(sheet, schema);
            Debug.Log(dataList.Count);

            foreach (Dictionary<string, object> iter in dataList)
            {
                Debug.LogFormat("{0},{1},{2}", iter["name"], iter["probability"], iter["items"]);
                List<float> items = iter["items"] as List<float>;
                string o = "";
                foreach (float s in items)
                {
                    o += s + ",";
                }

                Debug.Log(o);
            }
        }

        [MenuItem("Assets/Exl/Read Dict")]
        public static void TestReadDict()
        {
            string fileName = Application.dataPath + "/Tests/Editor/Gacha.xlsx";

            IWorkbook workbook = ExcelHelper.Load(fileName);
            ISheet sheet = workbook.GetSheetAt(0);
            Schema schema = EDSchemaReader.ReadSchema(sheet);
            EDDataReader reader = new EDDataReader();

            Dictionary<string,object> dataList = EDDataReader.ReadDictionary(sheet, schema);
            Debug.Log(dataList.Count);

            foreach (KeyValuePair<string, object> iter in dataList)
            {
                Dictionary<string,object> item = iter.Value as Dictionary<string, object>;

                Debug.LogFormat("{0},{1},{2}", item["name"], item["probability"], item["items"]);
                List<string> items = item["items"] as List<string>;
                string o = "";
                foreach (string s in items)
                {
                    o += s + ",";
                }

                Debug.Log(o);
            }
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

        [MenuItem("Assets/Exl/Code gen")]
        public static void TestCodeGen()
        {
            Field field = new Field("Test", ExcelDataType.Dictionary, "String,Object");
            Debug.Log(field.ToSystemType().GetGenericArguments()[0]);

            field = new Field("Test", ExcelDataType.List, "String");
            Debug.Log(field.ToSystemType().GetGenericArguments()[0]);
        }
    }
}