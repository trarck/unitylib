using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using YH;

namespace YHEditor
{
    public class ArrayField:BaseField
    {
        protected bool m_Foldout;

        protected List<BaseField> m_Elements;

        public ArrayField(object value, Type type,  string label):base(value, type,label)
        {

        }

        public ArrayField(object value, Type type, string label,int deep) : base(value, type, label,deep)
        {

        }

        public override void Init()
        {
            int len= m_Value==null?0: ReflectionUtils.GetLength(m_Value);
            m_Elements = new List<BaseField>();
            for(int i = 0; i < len; ++i)
            {
                m_Elements.Add(CreateElementField(i));
            }
        }

        public override void Draw()
        {
            
            int len = m_Elements.Count;

            m_Foldout = EditorGUILayout.Foldout(m_Foldout, m_Label);

            
            if (m_Foldout)
            {
                YHEditorTools.PushIndentLevel(m_Deep+1);
                YHEditorTools.PushLabelWidth(80);

                GUILayout.BeginVertical();
                //数组大小
                int newLen = EditorGUILayout.IntField("size", len);
                if (newLen != len)
                {
                    ChangeLength(newLen);
                }

                //数组元素
                for (int i = 0; i < m_Elements.Count; ++i)
                {
                    object oldElementValue = m_Elements[i].value;
                    m_Elements[i].Draw();
                    if (m_Elements[i].value != oldElementValue)
                    {
                        ReflectionUtils.InvokeMethod(m_Value, "SetValue", new object[] { m_Elements[i].value, i });
                    }
                }
                GUILayout.EndVertical();

                YHEditorTools.PopLabelWidth();
                YHEditorTools.PopIndentLevel();
            }
            
        }

        BaseField CreateElementField(int i)
        {
            object eleValue = ReflectionUtils.InvokeMethod(m_Value, "GetValue", new object[] { i });
            return BaseField.Create(eleValue, m_Type.GetElementType(), "Element " + i,m_Deep+1);
        }

        void ChangeLength(int newLen)
        {
            int oldLen = m_Elements.Count;

            //创建一个新的数组值
            object newValue = ReflectionUtils.InvokeConstructor(m_Type, new object[] { newLen });
            for (int i = 0; i < newLen; ++i)
            {
                //旧的元素复制到新元素上,超出直接截掉，不足用最后一个补。
                object ele = ReflectionUtils.InvokeMethod(m_Value, "GetValue", new object[] { i < oldLen ? i : oldLen - 1 });
                ReflectionUtils.InvokeMethod(newValue, "SetValue", new object[] { ele, i });
            }
            m_Value = newValue;

            //调整element list
            if (oldLen < newLen)
            {
                //添加
                for(int i = oldLen; i < newLen; ++i)
                {
                    m_Elements.Add(CreateElementField(i));
                }
            }
            else
            {
                //移除
                for(int i=oldLen-1;i>=newLen;--i)
                {
                    m_Elements.RemoveAt(i);
                }
            }
        }
    }
}