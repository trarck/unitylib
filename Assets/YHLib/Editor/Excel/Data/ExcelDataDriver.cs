using UnityEngine;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using System.Collections;
using YH.Data.Driver;

namespace TK.Excel
{
    /// <summary>
    /// Excel格式数据读取
    /// </summary>
    public class ExcelDataDriver : DataDriver
    {
        public HeadModel headModel = HeadModel.CreateNormalModel();

        public delegate string SheetNameFormatHandle(string excelFile);

        //通过excel名子来确定表名。
        public SheetNameFormatHandle sheetNameFormat=null;

        public ExcelDataDriver()
        {

        }

        public ExcelDataDriver(string dataPath)
        {
            m_DataPath = dataPath;
            m_DataFileExt = ".xlsx";
        }

        public override object FetchData(string name)
        {
            string file = ParseDataFileName(name);
            return LoadDataFromExcel(file,GetSheetName(name));
        }

        public override List<T> FetchData<T>(string name)
        {
            string file = ParseDataFileName(name);
            IList data= LoadDataFromExcel(file, GetSheetName(name));
            if (data != null)
            {
                return ExcelDataHelper.DeserializeData<T>(data);
            }
            return null;
        }

        public override IDictionary FetchDict(string name, string key)
        {
            string file = ParseDataFileName(name);
            return LoadDictFromExcel(file, GetSheetName(name), key);
        }

        public override Dictionary<K, T> FetchDict<K, T>(string name, string key)
        {
            if (typeof(K) == typeof(string))
            {
                IDictionary orignalDict = FetchDict(name,key);
                Dictionary<string, T> dict = new Dictionary<string, T>();
                foreach (IDictionaryEnumerator iter in orignalDict)
                {
                    dict[iter.Key.ToString()]= ExcelDataHelper.DeserializeObject<T>(iter.Value as Dictionary<string,object>);
                }
                return dict as Dictionary<K, T>;
            }
            else if (typeof(K) == typeof(int))
            {
                IDictionary orignalDict = FetchDict(name, key);
                Dictionary<int, T> dict = new Dictionary<int, T>();
                foreach (IDictionaryEnumerator iter in orignalDict)
                {
                    dict[(int)iter.Key] = ExcelDataHelper.DeserializeObject<T>(iter.Value as Dictionary<string, object>);
                }
                return dict as Dictionary<K, T>;
            }
            else if (typeof(K) == typeof(long))
            {
                IDictionary orignalDict = FetchDict(name,key);
                Dictionary<long, T> dict = new Dictionary<long, T>();
                foreach (IDictionaryEnumerator iter in orignalDict)
                {
                    dict[(long)iter.Key] = ExcelDataHelper.DeserializeObject<T>(iter.Value as Dictionary<string, object>);
                }
                return dict as Dictionary<K, T>;
            }
            return null;
        }

        public override void StoreData(object data,string name)
        {
            string file = ParseDataFileName(name);
            SaveDataToExcel(file, GetSheetName(name), data as IList);
        }

        public override void AppendData(object data, string name)
        {
            string file = ParseDataFileName(name);
            SaveDataToExcel(name, GetSheetName(name), data as IList);
        }



        #region Sheet

        public ISheet LoadSheet(string name)
        {
            string file = ParseDataFileName(name);
            return LoadExcel(file, GetSheetName(name));
        }

        public void SaveSheet(string name,ISheet sheet)
        {
            string file = ParseDataFileName(name);
            SaveExcel(file, sheet);
        }

        protected ISheet LoadExcel(string excelFile,string sheetName=null)
        {
            IWorkbook workbook = null;
            if (File.Exists(excelFile))
            {
                workbook = ExcelHelper.Load(excelFile);
            }
            else
            {
                string ext = Path.GetExtension(excelFile);
                TextAsset text = (TextAsset)Resources.Load(excelFile.Replace(ext, ""));
                if (text)
                {
                    MemoryStream stream = new MemoryStream(text.bytes);
                    workbook = ExcelHelper.Load(stream, excelFile.IndexOf(".xlsx") > -1);
                }
            }
            ISheet sheet = null;
            if (workbook != null)
            {
                //if sheetName empty use first sheet
                if (string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheetAt(0);
                }
                else
                {
                    sheet = workbook.GetSheet(sheetName);
                    //if the name error use first
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
            }
            return sheet;
        }

        protected void SaveExcel(string excelFile,ISheet sheet)
        {
            ExcelHelper.Save(excelFile, sheet.Workbook);
        }

        public IList LoadDataFromSheet(ISheet sheet)
        {
            if (sheet != null)
            {
                DataReader dataReader = new DataReader();
                dataReader.headModel = headModel;
                dataReader.side = Side.All;

                Schema schema = SchemaReader.ReadSchema(sheet, headModel);
                return dataReader.ReadList(sheet, schema, headModel);
            }
            return null;
        }

        public IDictionary LoadDictFromSheet(ISheet sheet, string keyField)
        {
            if (sheet != null)
            {
                DataReader dataReader = new DataReader();
                dataReader.headModel = headModel;
                dataReader.side = Side.All;

                Schema schema = SchemaReader.ReadSchema(sheet, headModel);
                return dataReader.ReadDictionary(sheet, schema, keyField, headModel);
            }
            return null;
        }

        public void SaveDataToSheet(ISheet sheet, IList data)
        {
            if (data != null && sheet != null)
            {
                DataWriter dataWriter = new DataWriter();
                dataWriter.side = Side.All;

                Schema schema = SchemaReader.ReadSchema(sheet, headModel);
                dataWriter.WriteList(sheet, schema, data, headModel);
            }
        }

        public void AppendDataToSheet(ISheet sheet, IList data)
        {
            if (data != null && sheet != null)
            {
                DataWriter dataWriter = new DataWriter();
                dataWriter.side = Side.All;

                Schema schema = SchemaReader.ReadSchema(sheet, headModel);
                dataWriter.WriteList(sheet, schema, data, sheet.LastRowNum + 1, 0, null);
            }
        }

        public bool UpdateSheet(ISheet sheet,Dictionary<string, object> conditions, Dictionary<string, object> data)
        {
            if (sheet == null)
            {
                Debug.LogError("[ExcelDataDriver:UpdateSheet] sheet is null");
                return false;
            }

            List<string> header = ReadHelper.GetHeader(sheet);
            Schema schema = SchemaReader.ReadSchema(sheet, headModel);

            List<Field> headerFields = ReadHelper.PrepareHeaderFields(header, schema);

            DataReader dataReader = new DataReader();
            dataReader.headModel = headModel;
            dataReader.side = Side.All;

            DataWriter dataWriter = new DataWriter();
            dataWriter.side = Side.All;

            bool dirty = false;

            int dataStart = headModel.DataRow;

            int l = sheet.LastRowNum;
            for (int i = sheet.FirstRowNum + dataStart; i <= l; ++i)
            {
                IRow row = sheet.GetRow(i);

                Dictionary<string, object> record = dataReader.ReadRowData(row, headerFields);

                bool match = true;

                foreach (var iter in conditions)
                {
                    if (!(record.ContainsKey(iter.Key) && iter.Value.Equals(record[iter.Key])))
                    {
                        match = false;
                        break;
                    }
                }

                //match conditions
                //foreach (var iter in data)
                //{
                //    record[iter.Key] = iter.Value;
                //}
                if (match)
                {
                    dataWriter.WriteRowDictionary(row, headerFields, data);
                    dirty = true;
                }
            }

            return dirty;
        }
        #endregion

        #region File
        IList LoadDataFromExcel(string excelFile,string sheetName)
        {
            ISheet sheet = LoadExcel(excelFile, sheetName);

            return LoadDataFromSheet(sheet);
        }

        IDictionary LoadDictFromExcel(string excelFile, string sheetName,string keyField)
        {
            ISheet sheet = LoadExcel(excelFile, sheetName);
            return LoadDictFromSheet(sheet,keyField);
        }

        void SaveDataToExcel(string excelFile, string sheetName, IList data)
        {
            if (data != null)
            {
                ISheet sheet = LoadExcel(excelFile,sheetName);
                if (sheet != null)
                {
                    DataWriter dataWriter = new DataWriter();
                    dataWriter.side = Side.All;

                    Schema schema = SchemaReader.ReadSchema(sheet, headModel);
                    dataWriter.WriteList(sheet, schema, data, headModel);
                    ExcelHelper.Save(excelFile, sheet.Workbook);
                }
            }
        }

        void AppendDataToExcel(string excelFile, IList data)
        {
            if (data != null)
            {
                ISheet sheet = LoadExcel(excelFile);
                if (sheet != null)
                {
                    DataWriter dataWriter = new DataWriter();
                    dataWriter.side = Side.All;

                    Schema schema = SchemaReader.ReadSchema(sheet, headModel);
                    dataWriter.WriteList(sheet, schema, data, sheet.LastRowNum + 1, 0, null);
                    ExcelHelper.Save(excelFile, sheet.Workbook);
                }
            }
        }

        public void UpdateExcel(Dictionary<string, object> conditions, Dictionary<string, object> data, string name)
        {
            string file = ParseDataFileName(name);
            ISheet sheet = LoadExcel(file, name);

            if (UpdateSheet(sheet, conditions, data))
            {
                ExcelHelper.Save(file, sheet.Workbook);
            }
        }

        #endregion

        public string GetSheetName(string name)
        {
            if (sheetNameFormat != null)
            {
                return sheetNameFormat(name);
            }
            return name + "Config";
        }
    }
}
