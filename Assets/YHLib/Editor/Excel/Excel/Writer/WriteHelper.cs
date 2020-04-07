using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace TK.Excel
{
    public class WriteHelper
    {
        #region cell
                
        public static void SetValue(ICell cell,int val)
        {
            if (cell != null)
            {
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue(val);
            }
        }
        public static void SetValue(ICell cell, long val)
        {
            if (cell != null)
            {
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue(val);
            }
        }

        public static void SetValue(ICell cell, float val)
        {
            if (cell != null)
            {
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue(val);
            }
        }

        public static void SetValue(ICell cell, double val)
        {
            if (cell != null)
            {
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue(val);
            }
        }

        public static void SetValue(ICell cell, bool val)
        {
            if (cell != null)
            {
                cell.SetCellType(CellType.Boolean);
                cell.SetCellValue(val);
            }
        }

        public static void SetValue(ICell cell, string val)
        {
            if (cell != null)
            {
                cell.SetCellType(CellType.String);
                cell.SetCellValue(val);
            }
        }

        public static void SetCellValue(ICell cell, object data, TypeInfo dataType)
        {
            if (data != null)
            {
                switch (dataType.sign)
                {
                    case TypeInfo.Sign.Byte:
                        SetValue(cell, System.Convert.ToByte(data));
                        break;
                    case TypeInfo.Sign.Int:
                        SetValue(cell, System.Convert.ToInt32(data));
                        break;
                    case TypeInfo.Sign.Long:
                        SetValue(cell, System.Convert.ToInt64(data));
                        break;
                    case TypeInfo.Sign.Float:
                        SetValue(cell, System.Convert.ToSingle(data));
                        break;
                      case TypeInfo.Sign.Double:
                        SetValue(cell, System.Convert.ToDouble(data));
                        break;
                    case TypeInfo.Sign.Boolean:
                        SetValue(cell, System.Convert.ToBoolean(data));
                        break;
                    case TypeInfo.Sign.String:
                        SetValue(cell, data.ToString());
                        break;
                    case TypeInfo.Sign.Array:
                    case TypeInfo.Sign.List:
                    case TypeInfo.Sign.Dictionary:
                    case TypeInfo.Sign.Generic:
                    case TypeInfo.Sign.Object:
                        SetValue(cell, Newtonsoft.Json.JsonConvert.SerializeObject(data));
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}