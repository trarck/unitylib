using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using YH;

namespace YHEditor
{
    
    public class TypeField:BaseField
    {
        BaseField m_Impl;

        public TypeField(object value, Type type, string label):base(value, type,label)
        {
            
        }

        public TypeField(object value, Type type, string label,int deep) : base(value, type, label,deep)
        {

        }

        public override void Init()
        {
            m_Impl = BaseField.Create(m_Value, m_Type, m_Label, m_Deep);
        }

        public override void Draw()
        {
            m_Impl.Draw();
            m_Value = m_Impl.value;
        }

        public override object value
        {
            get
            {
                return m_Impl!=null?m_Impl.value:null;
            }

            set
            {
                m_Value = value;

                if (m_Impl != null)
                {
                    m_Impl.value = value;
                }
            }
        }
    }
}