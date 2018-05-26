using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using YH;

namespace YHEditor
{
    
    public class ObjectField:BaseField
    {
        protected bool m_Foldout;
        protected List<BaseField> m_Elements;
        protected List<MemberInfo> m_Accesses;

        public ObjectField(object value, Type type, string label):base(value, type,label)
        {
            
        }

        public ObjectField(object value, Type type, string label,int deep) : base(value, type, label,deep)
        {

        }

        public override void Init()
        {
            m_Accesses = ReflectionUtils.GetAccessableFieldAndProperties(m_Type, false);
            m_Elements = new List<BaseField>();
            for (int i=0,l= m_Accesses.Count;i< l; ++i)
            {
                m_Elements.Add(CreateElementField(m_Accesses[i]));
            }
        }

        public override void Draw()
        {
            m_Foldout = EditorGUILayout.Foldout(m_Foldout, m_Label);

            if (m_Foldout)
            {
                YHEditorTools.PushIndentLevel(m_Deep + 1);
                YHEditorTools.PushLabelWidth(80);

                GUILayout.BeginVertical();

                //显示元素
                for (int i = 0; i < m_Elements.Count; ++i)
                {
                    object oldElementValue = m_Elements[i].value;
                    m_Elements[i].Draw();
                    if (m_Elements[i].value != oldElementValue)
                    {
                        ReflectionUtils.SetValue(m_Accesses[i], m_Value, m_Elements[i].value);
                    }
                }
                GUILayout.EndVertical();

                YHEditorTools.PopLabelWidth();
                YHEditorTools.PopIndentLevel();
            }
        }

        BaseField CreateElementField(MemberInfo info)
        {
            object eleValue = ReflectionUtils.GetValue(info, m_Value);
            return BaseField.Create(eleValue, ReflectionUtils.GetFieldOrPropertyType(info), info.Name, m_Deep + 1);
        }
    }
}