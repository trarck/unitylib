using YH.Data.Driver;

namespace YH.Data.Dao
{
    public class Dao:IDao
    {
        public virtual bool Init(IDataDriver dataDriver)
        {
            return true;
        }

        public bool Init(IDataDriver dataDriver, string tableName)
        {
            return true;
        }
    }
}