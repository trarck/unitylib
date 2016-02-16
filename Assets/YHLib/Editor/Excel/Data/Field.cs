using System.Collections.Generic;
using UnityEngine;

namespace YH.Excel.Data
{
    
    public class Field
    {
        //名称
        string m_Name;

        //数据类型
        ExcelDataType m_Type;

        //扩展类型
        string m_ExtType;

        //注释
        string m_Comment;

        public Field()
        {

        }

        public Field(string name,ExcelDataType type):this(name,type,"")
        {

        }

        public Field(string name, ExcelDataType type,string extType):this(name,type,extType,"")
        {
        }

        public Field(string name, ExcelDataType type, string extType,string comment)
        {
            m_Name = name;
            m_Type = type;
            m_ExtType = extType;
            m_Comment = comment;
        }

        public static ExcelDataType Parse(string type)
        {
            string extType;
            return Parse(type, out extType);
        }

        public static ExcelDataType Parse(string type, out string extType)
        {
            int pos = type.IndexOf("<");
            if (pos > -1)
            {
                string baseType = type.Substring(0, pos);
                int posEnd = type.IndexOf(">");
                
                extType = type.Substring(pos + 1, posEnd-pos-1);
                Debug.Log(pos + "," + posEnd+","+extType);
                return EnumUtil.ParseEnum<ExcelDataType>(baseType, ExcelDataType.Object);
            }
            else
            {
                extType = "";
                return EnumUtil.ParseEnum<ExcelDataType>(type, ExcelDataType.Object);
            }
        }


        public override string ToString()
        {
            return string.Format("Field[{0}]--{1}",m_Name ,m_Type);
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

        public string extType
        {
            set
            {
                m_ExtType = value;
            }

            get
            {
                return m_ExtType;
            }
        }

        public string comment
        {
            set
            {
                m_Comment = value;
            }

            get
            {
                return m_Comment;
            }
        }
    }
}