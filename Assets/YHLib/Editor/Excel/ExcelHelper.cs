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

        public static void Save(string fileName, IWorkbook workbook)
        {
            string ext = Path.GetExtension(fileName);

            if (workbook is HSSFWorkbook)
            {
                if (ext.ToLower() != ".xls")
                {
                    fileName = Path.GetDirectoryName(fileName) + Path.GetFileNameWithoutExtension(fileName) + ".xls";
                }
            }
            else if (workbook is XSSFWorkbook)
            {
                if (ext.ToLower() != ".xlsx")
                {
                    fileName = Path.GetDirectoryName(fileName) + Path.GetFileNameWithoutExtension(fileName) + ".xlsx";
                }
            }

            FileStream fs = File.Create(fileName);
            workbook.Write(fs);
            fs.Close();
        }
    }
}