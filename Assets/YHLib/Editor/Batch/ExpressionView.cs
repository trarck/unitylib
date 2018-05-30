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

        protected Vector2 m_ExpressionScrollPosition = Vector2.zero;

        protected string[] m_ExpressionNames;
        protected Dictionary<BatchExpression, BaseField> m_ValueFields = new Dictionary<BatchExpression, BaseField>();

        public BatchExpressionGroup m_Root;

        protected string m_Title;

        public bool isAny = true;

        protected ClassInfo m_ClassInfo;

        // Use this for initialization
        public virtual void Init(ClassInfo classInfo)
        {
            m_ClassInfo = classInfo;
        }

        public void OnGUI(Rect pos)
        {
            DrawExpressions();
        }

        public virtual void DrawExpressions()
        {

        }

        public virtual void DrawGroup(BatchExpressionGroup group,int deep)
        {
            if (group != null)
            {
                YHEditorTools.PushIndentLevel(deep);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(group.type.ToString());

                if (GUILayout.Button("+", GUILayout.Width(100)))
                {
                    AddExpressionToGroup(group);
                }

                if (GUILayout.Button("And", GUILayout.Width(100)))
                {
                    AddSubGroupToGroup(group, BatchExpressionGroup.GroupType.And);
                }

                if (GUILayout.Button("Or", GUILayout.Width(100)))
                {
                    AddSubGroupToGroup(group, BatchExpressionGroup.GroupType.Or);
                }

                if (GUILayout.Button("-", GUILayout.Width(100)))
                {
                    RemoveGroup(group);
                }

                EditorGUILayout.EndHorizontal();

                YHEditorTools.PushIndentLevel(deep + 1);
                if (group.expressions.Count > 0)
                {
                    for (int i = 0; i < group.expressions.Count; ++i)
                    {
                        DrawExpression(group.expressions[i]);
                    }
                }

                YHEditorTools.PopIndentLevel();

                if (group.subGroups.Count > 0)
                {
                    for (int i = 0; i < group.subGroups.Count; ++i)
                    {
                        DrawGroup(group.subGroups[i], deep + 1);
                    }
                }

                YHEditorTools.PopIndentLevel();
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

        protected void DrawExpressionValue(BatchExpression be)
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

        protected void AddExpressionToGroup(BatchExpressionGroup group)
        {
            BatchExpression expr = new BatchExpression();
            ChangeExpression(expr, group.expressions.Count, "");
            expr.parent = group;
            group.expressions.Add(expr);

        }

        protected void AddSubGroupToGroup(BatchExpressionGroup group, BatchExpressionGroup.GroupType subType)
        {
            BatchExpressionGroup subGroup = new BatchExpressionGroup();
            subGroup.type = subType;
            subGroup.parent = group;
            group.subGroups.Add(subGroup);
        }


        protected void RemoveGroup(BatchExpressionGroup group)
        {
            if (group.parent != null)
            {
                group.parent.subGroups.Remove(group);
            }
        }

        protected void ChangeExpression(BatchExpression be,int index,string name=null)
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
            if (be.parent != null)
            {
                be.parent.expressions.Remove(be);
            }

            m_ValueFields.Remove(be);
        }

        public void ChangeExpressionNames(string[] names,bool keep)
        {
            expressionNames = names;
            if (keep)
            {
                CheckExpressionNames(names,m_Root,m_Root);
            }
            else
            {
                if (m_Root != null)
                {
                    m_Root.Clear();
                }
            }
        }


        void CheckExpressionNames(string[] names,BatchExpressionGroup group, object a)
        {
            if (group != null)
            {
                if (group.expressions.Count > 0)
                {
                    for (int i = group.expressions.Count - 1; i >= 0; --i)
                    {
                        bool remove = true;
                        for (int j = 0; j < names.Length; ++j)
                        {
                            if (group.expressions[i].name == names[j])
                            {
                                group.expressions[i].index = j;
                                remove = false;
                                break;
                            }
                        }

                        if (remove)
                        {
                            group.expressions.RemoveAt(i);
                        }
                    }
                }

                for (int i = 0; i < group.subGroups.Count; ++i)
                {
                    CheckExpressionNames(names, group.subGroups[i],null);
                }
            }
        }

        public List<BatchExpression> GetNotNullExpressions()
        {
            if (m_Root != null)
            {
                return m_Root.GetNotNullExpressions();
            }
            return null;
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

        public BatchExpressionGroup root
        {
            get
            {
                return m_Root;
            }
        }
    }

    public class FindConditionView : ExpressionView
    {

        public FindConditionView()
        {
            title = "Conditions";
        }

        public override void DrawExpressions()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title);

            if (GUILayout.Button("And"))
            {
                CreateRoot(BatchExpressionGroup.GroupType.And);
            }

            if (GUILayout.Button("Or"))
            {
                CreateRoot(BatchExpressionGroup.GroupType.Or);
            }

            GUILayout.EndHorizontal();

            DrawGroup(m_Root, 0);
        }

        public override void DrawExpresstionOp(BatchExpression be)
        {
            be.op = (int)((FindOperation)EditorGUILayout.EnumPopup((FindOperation)be.op));
        }

        void CreateRoot(BatchExpressionGroup.GroupType type)
        {
            if (m_Root==null || m_Root.type != type)
            {
                m_Root = new BatchExpressionGroup();
                m_Root.type = type;
            }
        }
    }

    public class ModifyExpressionView : ExpressionView
    {
        public ModifyExpressionView()
        {
            title = "Expressions";
        }

        public override void Init(ClassInfo classInfo)
        {
            base.Init(classInfo);

            m_Root = new BatchExpressionGroup();
        }

        public override void DrawExpressions()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_Title);

            if (GUILayout.Button("+"))
            {
                AddExpressionToGroup(m_Root);
            }

            GUILayout.EndHorizontal();

            if (m_Root.expressions.Count > 0)
            {
                m_ExpressionScrollPosition = EditorGUILayout.BeginScrollView(m_ExpressionScrollPosition);

                for (int i = 0; i < m_Root.expressions.Count; ++i)
                {
                    DrawExpression(m_Root.expressions[i]);
                }

                EditorGUILayout.EndScrollView();
            }
        }

        public override void DrawExpresstionOp(BatchExpression be)
        {
            be.op = (int)((ModifyOperation)EditorGUILayout.EnumPopup((ModifyOperation)be.op));
        }
    }
}