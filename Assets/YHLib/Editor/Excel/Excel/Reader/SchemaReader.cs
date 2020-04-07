using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace TK.Excel
{
    public class SchemaReader
    {
        public static Schema ReadSchema(ISheet sheet, HeadModel headModel, int schemaColOffset = 0)
        {
            Schema schema = new Schema();
            schema.name = sheet.SheetName;

            //first row is name
            IRow NameRow = sheet.GetRow(sheet.FirstRowNum + headModel.NameRow);
            //second row is data type
            IRow typeRow = sheet.GetRow(sheet.FirstRowNum + headModel.DataTypeRow);

            bool haveDescription = headModel.DescriptionRow != -1;
            bool haveSide = headModel.SideRow != -1;
            //thirdrow is description
            IRow descriptionRow = null;
            if (haveDescription)
            {
                descriptionRow = sheet.GetRow(sheet.FirstRowNum + headModel.DescriptionRow);
            }

            IRow sideRow = null;
            if (haveSide)
            {
                sideRow = sheet.GetRow(sheet.FirstRowNum + headModel.SideRow);
            }

            for (int i = NameRow.FirstCellNum + schemaColOffset; i < NameRow.LastCellNum; ++i)
            {
                //get name
                ICell nameCell = NameRow.GetCell(i);
                string name = nameCell.StringCellValue;

                //get type
                TypeInfo dataType = TypeInfo.Object;
                ICell typeCell = typeRow.GetCell(i);
                if (typeCell != null)
                {
                    dataType = TypeInfo.Parse(typeCell.StringCellValue);
                }

                //get description
                string description = "";
                if (haveDescription)
                {
                    ICell descriptionCell = descriptionRow.GetCell(i);
                    description = descriptionCell.StringCellValue;
                }

                //get comment
                string comment = "";
                if (nameCell.CellComment != null)
                {
                    comment = nameCell.CellComment.String.String;
                }

                Side side = Side.All;
                if (haveSide)
                {
                    ICell sideCell = sideRow.GetCell(i);
                    string sideValue = ReadHelper.GetStringValue(sideCell);
                    if (!string.IsNullOrEmpty(sideValue))
                    {
                        side = (Side)System.Enum.Parse(typeof(Side), sideValue);
                    }
                }

                Field field = new Field(name, dataType, comment,description, side);
                schema.AddField(field);
            }

            return schema;
        }
    }
}