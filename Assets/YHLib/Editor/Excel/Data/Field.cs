using System.Collections.Generic;

namespace YH.Excel.Data
{
    public class Field
    {
        ExcelDataType m_Type;

        string m_Name;

        public Field()
        {

        }

        public Field(string name,ExcelDataType type)
        {
            m_Name = name;
            m_Type = type;
        }

        public ExcelDataType type
        {
            set
            {
                m_Type = value;
            }

            get
            {
                return m_Type;
            }
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
    }
}