using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using YH;

namespace YHEditor
{
    
    public class PrimitiveField:BaseField
    {

        public PrimitiveField(object value, Type type, string label):base(value, type,label)
        {
            
        }

        public PrimitiveField(object value, Type type, string label,int deep) : base(value, type, label,deep)
        {

        }

        public override void Init()
        {

        }

        public override void Draw()
        {
            YHEditorTools.PushIndentLevel(m_Deep);
            m_Value = YHGUI.DrawElement(m_Value, m_Type, m_Label);
            YHEditorTools.PopIndentLevel();
        }
    }
}