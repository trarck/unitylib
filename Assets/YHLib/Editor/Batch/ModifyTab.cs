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
        }

        void InitOperators()
        {

        }

        // Update is called once per frame
        public void Update(float delta)
        {
            
        }


        public void OnEnter()
        {
            m_ConditionNames = m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit);
        }

        public void OnExit()
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
                int index = EditorGUILayout.Popup(expr.index, m_ConditionNames);

                if (index == m_ConditionNames.Length - 1)
                {
                    expr.index = index;
                    //last one is custom define
                    string name = EditorGUILayout.TextField(expr.name);
                    if (expr.name != name)
                    {
                        ChangeExpresstion(expr, index, name);
                    }
                }
                else if (expr.index != index)
                {
                    ChangeExpresstion(expr, index, name);
                }
            }
            else
            {
                string name = EditorGUILayout.TextField(expr.name);
                if (expr.name != name)
                {
                    ChangeExpresstion(expr, 1, name);
                }
            }


            expr.op = (ModifyExpression.Operation)EditorGUILayout.EnumPopup(expr.op);

            DrawExpresstionValue(expr);

            if (GUILayout.Button("-"))
            {
                RemoveExpresstion(expr);
            }
            GUILayout.EndHorizontal();
        }

        void DrawExpresstionValue(ModifyExpression expr)
        {
            if (expr.type != null)
            {
                switch (expr.type.ToString())
                {
                    case "System.Int32":
                        expr.value = EditorGUILayout.IntField(expr.value != null ? (int)expr.value : 0);
                        break;
                    case "System.Int64":
                        expr.value = EditorGUILayout.LongField(expr.value != null ? (long)expr.value : 0);
                        break;
                    case "System.Single":
                        expr.value = EditorGUILayout.FloatField(expr.value != null ? (float)expr.value : 0);
                        break;
                    case "System.Double":
                        expr.value = EditorGUILayout.DoubleField(expr.value != null ? (double)expr.value : 0);
                        break;

                    case "UnityEngine.Vect2":
                        expr.value = EditorGUILayout.Vector2Field("", expr.value != null ? (Vector2)expr.value : Vector2.zero);
                        break;
                    case "UnityEngine.Vect3":
                        expr.value = EditorGUILayout.Vector3Field("", expr.value != null ? (Vector3)expr.value : Vector3.zero);
                        break;
                    case "UnityEngine.Vect4":
                        expr.value = EditorGUILayout.Vector4Field("", expr.value != null ? (Vector4)expr.value : Vector4.zero);
                        break;
                    case "UnityEngine.Rect":
                        expr.value = EditorGUILayout.RectField(expr.value != null ? (Rect)expr.value : Rect.zero);
                        break;
                    case "UnityEngine.Bounds":
                        expr.value = EditorGUILayout.BoundsField((Bounds)expr.value);
                        break;
                    case "UnityEngine.Color":
                        expr.value = EditorGUILayout.ColorField((Color)expr.value);
                        break;

                    case "UnityEngine.AnimationCurve":
                        expr.value = EditorGUILayout.CurveField((AnimationCurve)expr.value);
                        break;

                    default:

                        if (expr.type.IsSubclassOf(typeof(UnityEngine.Object)))
                        {
                            expr.value = EditorGUILayout.ObjectField((UnityEngine.Object)expr.value, expr.type, false);
                        }
                        else
                        {
                            expr.value = EditorGUILayout.TextField(expr.value != null ? expr.value.ToString() : "");
                        }
                        break;
                }
            }
        }

        void AddExpresstion()
        {
            ModifyExpression expr = new ModifyExpression();
            ChangeExpresstion(expr,m_ModifyExpressions.Count);
            m_ModifyExpressions.Add(expr);
        }

        void RemoveExpresstion(ModifyExpression expr)
        {
            m_ModifyExpressions.Remove(expr);
        }

        void ChangeExpresstion(ModifyExpression expr, int index, string name = null)
        {
            if (m_ConditionNames != null)
            {
                if (index < m_ConditionNames.Length - 1)
                {
                    expr.index = index;
                    expr.name = m_ConditionNames[expr.index];
                }
                else
                {
                    expr.index = m_ConditionNames.Length - 1;
                    expr.name = name;
                }
            }

            Type newType = m_Owner.controller.findClassInfo.GetMemberType(expr.name);
            if (newType != expr.type)
            {
                expr.type = newType;
                expr.value = null;
            }
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