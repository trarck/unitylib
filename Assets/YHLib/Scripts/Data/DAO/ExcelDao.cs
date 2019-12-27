using System.Collections.Generic;
using VO;
using NPOI.SS.UserModel;
using TK.Excel;
using System.Collections;
using System;
using System.Reflection;

namespace YH.Data.Dao
{
    public class ExcelDao
    {
        List<object> m_Data;
        ISheet m_Sheet;

        protected ExcelDataDriver m_Driver = null;
        protected string m_ExcelFile = null;
        
        public virtual void Init(ExcelDataDriver driver)
        {
            m_Driver = driver;
        }

        public virtual void Init(ExcelDataDriver driver,string excelFile)
        {
            m_Driver = driver;
            m_ExcelFile = excelFile;
        }

        public List<T> FetchAll<T>()
        {
            return ExcelDataHelper.DeserializeData<T>(data);
        }

        public virtual void Append(IList data)
        {
            m_Driver.AppendData(data, m_ExcelFile);
        }

        public void Update(Dictionary<string, object> conditions, Dictionary<string, object> data)
        {
            m_Driver.UpdateSheet(sheet,conditions, data);
        }

        public void RefreshData()
        {
            m_Data = m_Driver.LoadDataFromSheet(sheet) as List<object>;
        }

        public void Refresh()
        {
            //m_Data = m_Driver.FetchData(excelName) as List<object>;
            m_Sheet = m_Driver.LoadSheet(m_ExcelFile);
            m_Data = m_Driver.LoadDataFromSheet(m_Sheet) as List<object>;
        }

        public void Flush()
        {
            if (m_Sheet!=null)
            {
                m_Driver.SaveSheet(m_ExcelFile, m_Sheet);
            }
        }

        protected List<object> data
        {
            get
            {
                if (m_Data==null)
                {
                    m_Data = m_Driver.LoadDataFromSheet(sheet) as List<object>;
                }
                return m_Data;
            }
        }

        protected ISheet sheet
        {
            get
            {
                if (m_Sheet == null)
                {
                    m_Sheet = m_Driver.LoadSheet(m_ExcelFile);
                }
                return m_Sheet;
            }
        }

        protected virtual string excelName
        {
            get;
        }
    }
}