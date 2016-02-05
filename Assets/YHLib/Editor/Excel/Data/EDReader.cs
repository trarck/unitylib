using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

namespace YH.Excel.Data
{
    public class EDReader
    {
        IWorkbook m_Workbook;

        Schema m_Schema;

        int m_SchemaNameRow = 0;
        int m_SchemaDataTypeRow = 1;

        public EDReader(IWorkbook workbook)
        {
            m_Workbook = workbook;
        }

        public Schema ReadSchema(ISheet sheet)
        {
            Schema schema = new Schema();
            schema.name = sheet.SheetName;

            //first row is name
            IRow headerRow = sheet.GetRow(sheet.FirstRowNum + m_SchemaNameRow);

            //second row is data type
            IRow typeRow = sheet.GetRow(sheet.FirstRowNum + m_SchemaDataTypeRow);

            IEnumerator<ICell> headerIter = headerRow.GetEnumerator();
            IEnumerator<ICell> typeIter = typeRow.GetEnumerator();

            for(int i = headerRow.FirstCellNum; i < headerRow.LastCellNum; ++i)
            {
                string name = headerRow.GetCell(i).StringCellValue;
                

                ExcelDataType dataType = ExcelDataType.Object;
                string extType = "";

                ICell typeCell = typeRow.GetCell(i);
                if (typeCell!=null)
                {
                    dataType = Field.Parse(typeCell.StringCellValue, out extType);
                }

                string comment = "";
                if (headerIter.Current.CellComment != null)
                {
                    comment = headerIter.Current.CellComment.String.String;
                }

                Field field = new Field(name, dataType, extType, comment);
                schema.AddField(field);
            }

            return schema;
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