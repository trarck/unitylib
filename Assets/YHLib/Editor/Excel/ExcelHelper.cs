using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace YH
{
    public class ExcelHelper
    {
        public static IWorkbook Load(string fileName)
        {
            IWorkbook workbook = null;

            if (File.Exists(fileName))
            {
                //stream will close by workbook
                FileStream fs = File.OpenRead(fileName);

                if (fileName.IndexOf(".xlsx") > 0)// 新版
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileName.IndexOf(".xls") > 0)// 旧版
                {
                    workbook = new HSSFWorkbook(fs);
                }

                //FileStream is closed
            }

            return workbook;
        }
    }
}