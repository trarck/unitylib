using YH.Data.Driver;

namespace YH.Data.Dao
{
    public interface IDao
    {
        bool Init(IDataDriver dataDriver);
        bool Init(IDataDriver dataDriver,string name);
    }
}