using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace YH.Data.Driver
{
    /// <summary>
    /// 数据读取驱动。
    /// 无状态的可以多个Dao使用。有状态的只能一个Dao一个driver.
    /// </summary>
    public class DataDriver: IDataDriver
    {
        //数据存放的路径
        protected string m_DataPath;

        //数据文件的扩展名
        protected string m_DataFileExt = ".dat";

        public DataDriver()
        {

        }

        public DataDriver(string dataPath)
        {
            m_DataPath = dataPath;
        }
        #region List
        public virtual object FetchData(string name)
        {
            return null;
        }

        public virtual List<T> FetchData<T>(string name)
        {
            return null;
        }
        #endregion

        #region Dict
        public virtual IDictionary FetchDict(string name)
        {
            return null;
        }

        public virtual IDictionary FetchDict(string name, string key)
        {
            return null;
        }
        
        public virtual Dictionary<K, T> FetchDict<K, T>(string name)
        {
            return null;
        }

        public virtual Dictionary<K, T> FetchDict<K, T>(string name, string key)
        {
            return null;
        }

        #endregion

        #region Obj
        public virtual T FetchObject<T>(string name)
        {
            return default(T);
        }

        #endregion

        #region Write
        //Save
        public virtual void StoreData(object data,string name)
        {
            
        }
        //Append
        public virtual void AppendData(object data, string name)
        {
            
        }

        #endregion

        public virtual void Refresh()
        {

        }

        public virtual void Flush()
        {

        }

        protected virtual string ParseDataFileName(string fileName)
        {
            if (Path.IsPathRooted(fileName))
            {
                return fileName;
            }

            if (Path.GetExtension(fileName) != m_DataFileExt)
            {
                fileName += m_DataFileExt;
            }
            
            return Path.Combine(m_DataPath, fileName);
        }

        public string dataPath
        {
            set
            {
                m_DataPath = value;
            }

            get
            {
                return m_DataPath;
            }
        }

        public string dataFileExt
        {
            set
            {
                m_DataFileExt = value;
            }

            get
            {
                return m_DataFileExt;
            }
        }
    }
}
