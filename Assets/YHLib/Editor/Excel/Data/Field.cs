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

        public static ExcelDataType Parse(string type)
        {
            int pos = type.IndexOf("(");
            if (pos>-1)
            {
                string baseType = type.Substring(0, pos);
                int posEnd = type.IndexOf(")");
                string extType = type.Substring(pos+1,posEnd);

                return EnumUtil.ParseEnum<ExcelDataType>(baseType, ExcelDataType.Object);
            }
            else
            {
                return EnumUtil.ParseEnum<ExcelDataType>(type, ExcelDataType.Object);
            }            
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