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

        bool m_Inherit = false;

        string[] m_ConditionNames;

        Vector2 m_ExpresstionScrollPosition = Vector2.zero;

        List<ModifyExpression> m_ModifyExpressions = new List<ModifyExpression>();

        public string name { get; set; }
        
        // Use this for initialization
        public void Init(EditorTabs owner)
        {
            m_Owner = (BatchMain)owner;
            m_ConditionNames = m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit);
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

            bool inherit = EditorGUILayout.Toggle(m_Inherit);
            EditorGUILayout.LabelField("Inherit");

            if (m_Inherit != inherit)
            {
                m_Inherit = inherit;
                ChangeInherit();
            }

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

            if (m_ConditionNames != null && m_ConditionNames.Length > 0)
            {
                expr.index = EditorGUILayout.Popup(expr.index, m_ConditionNames);
                if (expr.index == m_ConditionNames.Length - 1)
                {
                    //last one is custom define
                    expr.name = EditorGUILayout.TextField(expr.name);
                }
                else
                {
                    expr.name = m_ConditionNames[expr.index];
                }
            }
            else
            {
                expr.name = EditorGUILayout.TextField(expr.name);
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


        void ChangeInherit()
        {
            m_ConditionNames = m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit);

            List<ModifyExpression> keeps = new List<ModifyExpression>();

            for (int i = 0; i < m_ModifyExpressions.Count; ++i)
            {
                for (int j = 0; j < m_ConditionNames.Length; ++j)
                {
                    if (m_ModifyExpressions[i].name == m_ConditionNames[j])
                    {
                        keeps.Add(m_ModifyExpressions[i]);
                        break;
                    }
                }
            }
            m_ModifyExpressions = keeps;
        }

        void DoModify()
        {
            m_Owner.controller.Modify(m_ModifyExpressions);
        }
    }
}