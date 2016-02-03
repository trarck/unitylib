using System.Collections.Generic;

namespace YH.Excel.Data
{

    public class Schema
    {
        string m_Name;

        List<Field> m_Fields;

        public Schema()
        {
            m_Fields = new List<Field>();
        }

        public void AddField(string name, ExcelDataType type)
        {
            if (!Exists(name))
            {
                Field field = new Field(name, type);
                m_Fields.Add(field);
            }
        }

        public void AddField(string name, ExcelDataType type,string extType)
        {
            if (!Exists(name))
            {
                Field field = new Field(name, type,extType);
                m_Fields.Add(field);
            }
        }

        public void AddField(Field field)
        {
            if (!Exists(field.name))
            {
                m_Fields.Add(field);
            }
        }

        public void RemoveField(string name,ExcelDataType type)
        {
            for (int i=0,l=m_Fields.Count;i< l;++i)
            {
                if (m_Fields[i].name == name)
                {
                    m_Fields.RemoveAt(i);
                    break;
                }
            }
        }

        public void UpdateField(string name, ExcelDataType newType)
        {
            for (int i = 0, l = m_Fields.Count; i < l; ++i)
            {
                if (m_Fields[i].name == name)
                {
                    m_Fields[i].type=newType;
                    break;
                }
            }
        }

        public bool Exists(string name)
        {
            foreach (Field field in m_Fields)
            {
                if (field.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public string name
        {
            set
            {
                m_Name = value;
            }

            get
            {
                return m_Name;
            }
        }

        public List<Field> fields
        {
            set
            {
                m_Fields = value;
            }

            get
            {
                return m_Fields;
            }
        }
    }
}