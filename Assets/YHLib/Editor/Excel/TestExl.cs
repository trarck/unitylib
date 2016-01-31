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
            string fileName = Application.dataPath + "/Tests/Editor/Gacha.xlsx";

            IWorkbook workbook = ExcelHelper.Load(fileName);
            EDReader reader = new EDReader(workbook);

            ISheet dataSheet = workbook.GetSheetAt(0);

            Schema schema = reader.ReadSchema(dataSheet);

            foreach(Field field in schema.fields)
            {
                Debug.Log(field);
            }
        }
    }
}