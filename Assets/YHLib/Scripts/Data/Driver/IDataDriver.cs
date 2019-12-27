using System.Collections;
using System.Collections.Generic;

namespace YH.Data.Driver
{
    public interface IDataDriver
    {
        object FetchData(string name);

        List<T> FetchData<T>(string name);

        IDictionary FetchDict(string name);
        IDictionary FetchDict(string name, string key);

        Dictionary<K,T> FetchDict<K,T>(string name);
        Dictionary<K, T> FetchDict<K, T>(string name, string key);

        T FetchObject<T>(string name);

        void StoreData(object data,string name);

        void AppendData(object data, string name);

        void Refresh();

        void Flush();
    }
}
