using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace YH
{
    public class ModifyTab:IEditorTab
    {
        BatchMain m_Owner;

        Vector2 m_ExpresstionScrollPosition = Vector2.zero;

        List<ModifyExpression> m_ModifyExpressions = new List<ModifyExpression>();

        public string name { get; set; }
        
        // Use this for initialization
        public void Init(EditorTabs owner)
        {
            m_Owner = (BatchMain)owner;
        }

        void InitOperators()
        {

        }

        // Update is called once per frame
        public void Update(float delta)
        {
            
        }

        public void OnGUI(Rect pos)
        {
            EditorGUILayout.Space();

            DrawExpressions();
            if (GUILayout.Button("Modify"))
            {
                DoModify();
            }
        }


        void DrawExpressions()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Conditions");

            if (GUILayout.Button("+"))
            {
                AddExpresstion();
            }

            GUILayout.EndHorizontal();

            if (m_ModifyExpressions != null && m_ModifyExpressions.Count > 0)
            {
                m_ExpresstionScrollPosition = EditorGUILayout.BeginScrollView(m_ExpresstionScrollPosition);

                for (int i = 0; i < m_ModifyExpressions.Count; ++i)
                {
                    DrawExpression(m_ModifyExpressions[i]);
                }

                EditorGUILayout.EndScrollView();
            }
        }


        void DrawExpression(ModifyExpression expr)
        {
            GUILayout.BeginHorizontal();
            ClassInfo findClassInfo = m_Owner.controller.findClassInfo;

            if (findClassInfo.fieldNames != null && findClassInfo.fieldNames.Length > 0)
            {
                expr.fieldIndex = EditorGUILayout.Popup(expr.fieldIndex, findClassInfo.fieldNames);
                if (expr.fieldIndex == findClassInfo.fieldNames.Length - 1)
                {
                    //last one is custom define
                    expr.field = EditorGUILayout.TextField(expr.field);
                }
                else
                {
                    expr.field = findClassInfo.fieldNames[expr.fieldIndex];
                }
            }
            else
            {
                expr.field = EditorGUILayout.TextField(expr.field);
            }


            expr.op = (ModifyExpression.Operation)EditorGUILayout.EnumPopup(expr.op);
            expr.value = EditorGUILayout.TextField(expr.value);
            if (GUILayout.Button("-"))
            {
                RemoveExpresstion(expr);
            }
            GUILayout.EndHorizontal();
        }

        void AddExpresstion()
        {
            ModifyExpression expr = new ModifyExpression();
            m_ModifyExpressions.Add(expr);
        }

        void RemoveExpresstion(ModifyExpression expr)
        {
            m_ModifyExpressions.Remove(expr);
        }

        void DoModify()
        {
            m_Owner.controller.Modify(m_ModifyExpressions);
        }
    }
}