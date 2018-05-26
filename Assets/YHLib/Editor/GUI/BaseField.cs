using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using YH;

namespace YHEditor
{

    //value保存在内部
    public class BaseField
    {
        protected string m_Label;
        protected object m_Value;
        protected Type m_Type;
        protected int m_Deep=0;


        public BaseField()
        {

        }

        public BaseField(object value, Type type, string label)
        {
            m_Value = value;
            m_Type = type;
            m_Label = label;
            m_Deep = 0;
        }

        public BaseField(object value, Type type, string label, int deep)
        {
            m_Value = value;
            m_Type = type;
            m_Label = label;
            m_Deep = deep;
        }


        public virtual void Init()
        {

        }

        public virtual void Draw()
        {
        }

        public virtual object value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        public static BaseField Create(object value,Type type,string label,int deep=0)
        {
            if (type.IsArray)
            {
                ArrayField arrField = new ArrayField(value, type, label,deep);
                arrField.Init();
                return arrField;
            }
            else
            {
                ObjectField objField = new ObjectField(value, type, label,deep);
                objField.Init();
                return objField;
            }
        }
    }
}