using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using YHEditor;

namespace YH
{
    
    public class ExpressionView
    {

        Vector2 m_ExpressionScrollPosition = Vector2.zero;

        string[] m_ExpressionNames;

        List<BatchExpression> m_Expressions = new List<BatchExpression>();
        Dictionary<BatchExpression ,BaseField> m_ValueFields = new Dictionary<BatchExpression, BaseField>();

        string m_Title;

        public bool isAny = true;

        ClassInfo m_ClassInfo;

        // Use this for initialization
        public void Init(ClassInfo classInfo)
        {
            m_ClassInfo = classInfo;
        }

        public void OnGUI(Rect pos)
        {
            DrawExpressions();
        }

        public virtual void DrawExpressions()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_Title);

            isAny = EditorGUILayout.Toggle("Any", isAny);

            if (GUILayout.Button("+"))
            {
                AddExpression();
            }

            GUILayout.EndHorizontal();

            if (m_Expressions != null && m_Expressions.Count > 0)
            {
                m_ExpressionScrollPosition = EditorGUILayout.BeginScrollView(m_ExpressionScrollPosition);

                for (int i = 0; i < m_Expressions.Count; ++i)
                {
                    DrawExpression(m_Expressions[i]);
                }

                EditorGUILayout.EndScrollView();
            }
        }

        public virtual void DrawExpressionName(BatchExpression be)
        {
            if (m_ExpressionNames != null && m_ExpressionNames.Length > 0)
            {
                int index = EditorGUILayout.Popup(be.index, m_ExpressionNames);

                if (index == m_ExpressionNames.Length - 1)
                {
                    be.index = index;
                    //last one is custom define
                    string name = EditorGUILayout.TextField(be.name);
                    if (be.name != name)
                    {
                        ChangeExpression(be, index, name);
                    }
                }
                else if (be.index != index)
                {
                    ChangeExpression(be, index, "");
                }
            }
            else
            {
                string name = EditorGUILayout.TextField(be.name);
                if (be.name != name)
                {
                    ChangeExpression(be, 1, name);
                }
            }
        }

        public virtual void DrawExpresstionOp(BatchExpression be)
        {
            //
        }

        public virtual void DrawExpression(BatchExpression be)
        {
            GUILayout.BeginHorizontal();

            DrawExpressionName(be);

            DrawExpresstionOp(be);

            DrawExpressionValue(be);

            if (GUILayout.Button("-"))
            {
                RemoveExpression(be);
            }

            GUILayout.EndHorizontal();
        }

        void DrawExpressionValue(BatchExpression be)
        {
            if (m_ValueFields.ContainsKey(be))
            {
                BaseField baseField = m_ValueFields[be];
                baseField.Draw();
                if (baseField.value != be.value)
                {
                    be.value = baseField.value;
                }
            }
        }

        void AddExpression()
        {
            BatchExpression be = new BatchExpression();
            ChangeExpression(be,m_Expressions.Count,"");
            m_Expressions.Add(be);
        }

        void ChangeExpression(BatchExpression be,int index,string name=null)
        {
            if (m_ExpressionNames != null)
            {
                if (index < m_ExpressionNames.Length - 1)
                {
                    be.index = index;
                    be.name = m_ExpressionNames[be.index];
                }
                else
                {
                    be.index = m_ExpressionNames.Length - 1;
                    be.name = name;
                }
            }
            else
            {
                be.index = 0;
                be.name = name;
            }

            Type newType = m_ClassInfo.GetMemberType(be.name);
            if (newType != be.type)
            {
                be.type = newType;
                be.value = null;

                m_ValueFields[be]=BaseField.Create(be.value, be.type, "");
            }

            if (!m_ValueFields.ContainsKey(be) && be.type!=null)
            {
                m_ValueFields.Add(be, BaseField.Create(be.value, be.type, ""));
            }
        }

        public void RemoveExpression(BatchExpression be)
        {
            m_Expressions.Remove(be);
            m_ValueFields.Remove(be);
        }

        public void ChangeExpressionNames(string[] names,bool keep)
        {
            expressionNames = names;
            if (keep)
            {
                for (int i = m_Expressions.Count-1;i>=0; --i)
                {
                    bool remove = true;
                    for (int j = 0; j < m_ExpressionNames.Length; ++j)
                    {
                        if (m_Expressions[i].name == m_ExpressionNames[j])
                        {
                            m_Expressions[i].index = j;
                            remove = false;
                            break;
                        }
                    }

                    if (remove)
                    {
                        m_Expressions.RemoveAt(i);
                    }
                }
            }
            else
            {
                m_Expressions.Clear();
            }
        }

        public List<BatchExpression> GetNotNullExpressions()
        {
            List<BatchExpression> keeps = new List<BatchExpression>();
            for (int i = 0; i < m_Expressions.Count; ++i)
            {
                if (m_Expressions[i].value != null)
                {
                    keeps.Add(m_Expressions[i]);
                }
            }
            return keeps;
        }        

        public string title
        {
            get
            {
                return m_Title;
            }
            set
            {
                m_Title = value;
            }
        }

        public string[] expressionNames
        {
            get
            {
                return m_ExpressionNames;
            }
            set
            {
                m_ExpressionNames = value;  
            }
        }

        public ClassInfo classInfo
        {
            set
            {
                m_ClassInfo = value;
            }
            get
            {
                return m_ClassInfo;
            }
        }
    }

    public class FindConditionView:ExpressionView
    {
        public FindConditionView ()
        {
            title = "Conditions";
        }

        public override void DrawExpresstionOp(BatchExpression be)
        {
            be.op = (int)((FindOperation)EditorGUILayout.EnumPopup((FindOperation)be.op));
        }
    }

    public class ModifyExpressionView : ExpressionView
    {
        public ModifyExpressionView()
        {
            title = "Expressions";
        }

        public override void DrawExpresstionOp(BatchExpression be)
        {
            be.op = (int)((ModifyOperation)EditorGUILayout.EnumPopup((ModifyOperation)be.op));
        }
    }
}