using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace TK.Excel
{
    public class SchemaWriter
    {
        public static void WriteSchema(Schema schema,ISheet sheet, HeadModel headModel, int schemaColOffset = 0)
        {
            if(schema==null || sheet == null)
            {
                return;
            }

            //first row is name
            IRow NameRow = sheet.CreateRow(sheet.FirstRowNum + headModel.NameRow);
            //second row is data type
            IRow typeRow = sheet.CreateRow(sheet.FirstRowNum + headModel.DataTypeRow);

            bool haveDescription = headModel.DescriptionRow != -1;
            bool haveSide = headModel.SideRow != -1;
            //third row is description
            IRow descriptionRow = null;
            if (haveDescription)
            {
                descriptionRow = sheet.CreateRow(sheet.FirstRowNum + headModel.DescriptionRow);
            }

            IRow sideRow = null;
            if (haveSide)
            {
                sideRow = sheet.CreateRow(sheet.FirstRowNum + headModel.SideRow);
            }

            for(int i = 0, l = schema.fields.Count; i < l; ++i)
            {
                int col = i + schemaColOffset;
                Field field = schema.fields[i];
                //name
                ICell nameCell = NameRow.CreateCell(col,CellType.String);
                nameCell.SetCellValue(field.name);
  
                //type cell
                ICell typeCell = typeRow.CreateCell(col,CellType.String);
                typeCell.SetCellValue(field.type.ToString());

                //description
                if (haveDescription)
                {
                    ICell descriptionCell = descriptionRow.CreateCell(col, CellType.String);
                    descriptionCell.SetCellValue(field.description);
                }

                //side
                if (haveSide)
                {
                    ICell sideCell = sideRow.CreateCell(col,CellType.String);
                    sideCell.SetCellValue(field.side.ToString());
                }
            }
        }
    }
}