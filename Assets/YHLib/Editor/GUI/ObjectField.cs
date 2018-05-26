using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using YH;

namespace YHEditor
{
    
    public class ObjectField:BaseField
    {

        public ObjectField(object value, Type type, string label):base(value, type,label)
        {
            
        }

        public ObjectField(object value, Type type, string label,int deep) : base(value, type, label,deep)
        {

        }

        public override void Init()
        {
            if (m_Type.IsArray)
            {
                throw new Exception("Object Field type is array");
            }
        }

        public override void Draw()
        {
            YHEditorTools.PushIndentLevel(m_Deep);
            m_Value = YHGUI.DrawElement(m_Value, m_Type, m_Label);
            YHEditorTools.PopIndentLevel();
        }
    }
}