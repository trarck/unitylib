using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

namespace YH.Excel.Data
{
    public class EDDataReader
    {
        IWorkbook m_Workbook;

        Schema m_Schema;

        int m_SchemaNameRow = 0;
        int m_SchemaDataTypeRow = 1;

        public EDDataReader()
        {

        }

        public EDDataReader(IWorkbook workbook,Schema schema)
        {
            m_Workbook = workbook;
            m_Schema = schema;
        }

        public Dictionary<string,object> ReadDictionary(ISheet sheet,int dataStartOffset=0)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            for(int i = sheet.FirstRowNum + dataStartOffset; i < sheet.LastRowNum; ++i)
            {

            }

            return data;
        }

        Dictionary<string, object> ReadRowData(IRow row)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            IEnumerator<ICell> iter = row.GetEnumerator();
            while (iter.MoveNext())
            {

            }

            return data;
        }

        public Schema schema
        {
            set
            {
                m_Schema = value;
            }

            get
            {
                return m_Schema;
            }
        }

        public IWorkbook workbook
        {
            set
            {
                m_Workbook = value;
            }

            get
            {
                return m_Workbook;
            }
        }
    }
}