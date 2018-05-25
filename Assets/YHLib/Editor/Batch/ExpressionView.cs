using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace YH
{
    
    public class ExpressionView
    {

        Vector2 m_ExpressionScrollPosition = Vector2.zero;

        string[] m_ExpressionNames;

        List<BatchExpression> m_Expressions = new List<BatchExpression>();

        string m_Title;

        Batch m_Controller;

        // Use this for initialization
        public void Init(Batch controller)
        {
            m_Controller = controller;
        }

        public void OnGUI(Rect pos)
        {
            DrawExpressions();
        }

        public virtual void DrawExpressions()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_Title);

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
            if (be.type != null)
            {
                switch (be.type.ToString())
                {
                    case "System.Int32":
                        be.value = EditorGUILayout.IntField(be.value!=null?(int)be.value:0);
                        break;
                    case "System.Int64":
                        be.value = EditorGUILayout.LongField(be.value != null?(long)be.value:0);
                        break;
                    case "System.Single":
                        be.value = EditorGUILayout.FloatField(be.value != null ? (float)be.value:0);
                        break;
                    case "System.Double":
                        be.value = EditorGUILayout.DoubleField(be.value != null ? (double)be.value:0);
                        break;

                    case "UnityEngine.Vect2":
                        be.value = EditorGUILayout.Vector2Field("", be.value != null ? (Vector2)be.value:Vector2.zero);
                        break;
                    case "UnityEngine.Vect3":
                        be.value = EditorGUILayout.Vector3Field("", be.value != null ? (Vector3)be.value:Vector3.zero);
                        break;
                    case "UnityEngine.Vect4":
                        be.value = EditorGUILayout.Vector4Field("", be.value != null ? (Vector4)be.value:Vector4.zero);
                        break;
                    case "UnityEngine.Rect":
                        be.value = EditorGUILayout.RectField(be.value != null ? (Rect)be.value:Rect.zero);
                        break;
                    case "UnityEngine.Bounds":
                        be.value = EditorGUILayout.BoundsField(be.value != null ? (Bounds)be.value:new Bounds(Vector3.zero,Vector3.zero));
                        break;
                    case "UnityEngine.Color":
                        be.value = EditorGUILayout.ColorField(be.value != null ? (Color)be.value:Color.black);
                        break;

                    case "UnityEngine.AnimationCurve":
                        be.value = EditorGUILayout.CurveField((AnimationCurve)be.value);
                        break;

                    default:

                        if (be.type.IsSubclassOf(typeof(UnityEngine.Object)))
                        {
                            be.value = EditorGUILayout.ObjectField((UnityEngine.Object)be.value, be.type, false);
                        }
                        else
                        {
                            be.value = EditorGUILayout.TextField(be.value!=null?be.value.ToString():"");
                        }
                        break;
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

            Type newType = m_Controller.findClassInfo.GetMemberType(be.name);
            if (newType != be.type)
            {
                be.type = newType;
                be.value = null;
            }
        }

        public void RemoveExpression(BatchExpression be)
        {
            m_Expressions.Remove(be);
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