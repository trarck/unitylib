using System.Collections.Generic;

namespace TK.Excel
{

    public class Schema
    {
        string m_Name;

        List<Field> m_Fields;

        public Schema()
        {
            m_Fields = new List<Field>();
        }

        public void AddField(string name, TypeInfo type)
        {
            if (!Exists(name))
            {
                Field field = new Field(name, type);
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

        public void RemoveField(string name,TypeInfo type)
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

        public void UpdateField(string name, TypeInfo newType)
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

        public Field GetField(string name)
        {
            foreach (Field field in m_Fields)
            {
                if (field.name == name)
                {
                    return field;
                }
            }
            return null;
        }

        public List<Field> GetSideFields(Side side)
        {
            List<Field> sideFields = new List<Field>();
            foreach (Field field in m_Fields)
            {
                if (field.IsSameSide(side))
                {
                    sideFields.Add(field);
                }
            }

            return sideFields;
        }

        string CamelCase(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                char[] chars = str.ToCharArray();
                int l = chars.Length;
                int j = 0;
                int i = 0;
                //check first char
                char c = chars[i];
                if (c >= 'a' && c <= 'z')
                {
                    chars[i] = char.ToUpper(c);
                }
                else if (c == '_')
                {
                    ++i;
                }

                for (; i < l; ++i, ++j)
                {
                    if (chars[i] == '_')
                    {
                        if (i + 1 < chars.Length)
                        {
                            c = chars[i + 1];

                            if (c >= 'a' && c <= 'z')
                            {
                                chars[i + 1] = char.ToUpper(c);
                            }

                            if (c != '_')
                            {
                                ++i;
                            }
                        }
                    }

                    if (j != i)
                    {
                        chars[j] = chars[i];
                    }
                }


                str = new string(chars, 0, j);
            }
            return str;
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

        public string className
        {
            get
            {
                if (!string.IsNullOrEmpty(m_Name))
                {
                    return CamelCase(m_Name);
                }

                return m_Name;
            }
        }

        public Side side
        {
            get
            {
                Side side = Side.None;
                foreach(var field in m_Fields)
                {
                    side |= field.side;
                }
                return side;
            }
        }
    }
}