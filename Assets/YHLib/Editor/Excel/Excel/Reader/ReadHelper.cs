using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace TK.Excel
{
    public class ReadHelper
    {
        #region header
        public static List<string> GetHeader(ISheet sheet, int headerOffset = 0,int colStart=0,int colEnd=-1)
        {
            List<string> header = new List<string>();

            //first row is header as default
            IRow headerRow = sheet.GetRow(sheet.FirstRowNum + headerOffset);
            int l = colEnd < 1 ? headerRow.LastCellNum : (colEnd < headerRow.LastCellNum ? colEnd : headerRow.LastCellNum);
            for(int i=headerRow.FirstCellNum+colStart;i<l;++i)
            {
                header.Add(headerRow.GetCell(i).StringCellValue);
            }
            return header;
        }

        public static List<Field> PrepareHeaderFields(List<string> header, Schema schema)
        {
            List<Field> headerFields = new List<Field>();
            foreach (string name in header)
            {
                headerFields.Add(schema.GetField(name));
            }

            return headerFields;
        }
        #endregion

        #region cell
                
        public static int GetIntValue(ICell cell)
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                    case CellType.Formula:
                        return (int)cell.NumericCellValue;
                    case CellType.String:
                        return int.Parse(cell.StringCellValue);
                    case CellType.Boolean:
                        return cell.BooleanCellValue ? 1 : 0;
                    default:
                        int i;
                        int.TryParse(cell.StringCellValue,out i);
                        return i;
                }
            }
            else
            {
                return 0;
            }
        }

        public static long GetLongValue(ICell cell)
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                    case CellType.Formula:
                        return (long)cell.NumericCellValue;
                    case CellType.String:
                        return long.Parse(cell.StringCellValue);
                    case CellType.Boolean:
                        return cell.BooleanCellValue ? 1 : 0;
                    default:
                        throw new System.Exception("can't convert to long from " + cell.CellType);
                }
            }
            else
            {
                return 0;
            }
        }

        public static float GetFloatValue(ICell cell)
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                    case CellType.Formula:
                        return (float)cell.NumericCellValue;
                    case CellType.String:
                        return float.Parse(cell.StringCellValue);
                    default:
                        throw new System.Exception("can't convert to float from " + cell.CellType);
                }
            }
            else
            {
                return 0;
            }
        }

        public static double GetDoubleValue(ICell cell)
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                    case CellType.Formula:
                        return cell.NumericCellValue;
                    case CellType.String:
                        return double.Parse(cell.StringCellValue);
                    default:
                        throw new System.Exception("can't convert to double from " + cell.CellType);
                }
            }
            else
            {
                return 0;
            }
        }

        public static bool GetBoolValue(ICell cell)
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                    case CellType.Formula:
                        return cell.NumericCellValue != 0;
                    case CellType.String:
                        return bool.Parse(cell.StringCellValue);
                    case CellType.Boolean:
                        return cell.BooleanCellValue;
                    default:
                        throw new System.Exception("can't convert to bool from " + cell.CellType);
                }
            }
            else
            {
                return false;
            }
        }

        public static string GetStringValue(ICell cell)
        {
            if (cell!=null)
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                    case CellType.Formula:
                        return cell.NumericCellValue.ToString();
                    case CellType.Boolean:
                        return cell.BooleanCellValue.ToString();
                    default:
                        return cell.StringCellValue;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 格式：__XXX;xxx!A1F12;xxx!1F1;xxx!A1,2;xxx!,D2
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static bool IsLinkCell(ICell cell)
        {
            if (cell != null)
            {
                return cell.StringCellValue.StartsWith("__") || cell.StringCellValue.IndexOf("!") > -1;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}