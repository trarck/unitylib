using UnityEngine;
using System.Collections.Generic;
using YH;
using YH.Data.Dao;
using YH.Data.Driver;

public class UserDao : DataDao
{
    Dictionary<string,object> m_Data;

    public UserDao(DataDriver dataDriver)
    {
        m_DataDriver = dataDriver;

        m_Data = m_DataDriver.FetchData("User") as Dictionary<string,object>;
    }

    public List<Dictionary<string,object>> GetAllMale()
    {
        List<Dictionary<string,object>> males=new List<Dictionary<string,object>>();
        foreach (KeyValuePair<string, object> iter in this.data)
        {
            Dictionary<string, object> user = iter.Value as Dictionary<string, object>;

            if ((int)user["Sex"] == 0)
            {
                males.Add(user);
            }
        }

        return males;
    }

    public Dictionary<string, object> data
    {
        set
        {
            m_Data = value;
        }

        get
        {
            if (m_Data == null)
            {
                m_Data = m_DataDriver.FetchData("User") as Dictionary<string, object>;
            }
            return m_Data;
        }
    }

}
