namespace TK.Excel
{
    public class Field
    {
        //名称
        string m_Name;

        //描述
        string m_Description;

        //数据类型
        TypeInfo m_Type;

        //注释
        string m_Comment;

        //side
        Side m_Side;

        public Field()
        {

        }

        public Field(string name,TypeInfo type)
        {
            m_Name = name;
            m_Type = type;
            m_Side = Side.All;
        }
        
        public Field(string name, TypeInfo type, string comment,string description,Side side)
        {
            m_Name = name;
            m_Type = type;
            m_Comment = comment;
            m_Description = description;
            m_Side = side;
        }

        public override string ToString()
        {
            return string.Format("Field[{0}]--{1}",m_Name ,m_Type);
        }

        public bool IsSameSide(Side side)
        {
            return (m_Side & side) != 0;
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

        public TypeInfo type
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

        public string description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
            }
        }

        public Side side
        {
            get
            {
                return m_Side;
            }
            set
            {
                m_Side = value;
            }
        }
    }
}