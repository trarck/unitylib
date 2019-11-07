using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using YH;
using UnityEngine.Events;

namespace YHEditor
{
    [System.Serializable]
    public class ModifyTab : IEditorTab
    {
        public enum ActionType
        {
            //执行表达式
            Express,
            //执行函数
            Function
        }

        BatchMain m_Owner;
        string m_ClassName;
        bool m_Inherit = false;
        ActionType m_ActionType;

        ModifyExpressionView m_ModifyExpressionView;

        string[] m_ClassMethodNames=new string[0];
        List<MethodInfo> m_ClassMethods = new List<MethodInfo>();
        int m_SelectMethodIndex = 0;


        List<BaseField> m_Parameters = new List<BaseField>();

        public string name { get; set; }

        // Use this for initialization
        public void Init(EditorTabs owner)
        {
            m_Owner = (BatchMain)owner;
            m_ModifyExpressionView = new ModifyExpressionView();
            m_ModifyExpressionView.Init(m_Owner.controller.findClassInfo);
        }

        // Update is called once per frame
        public void Update(float delta)
        {

        }


        public void OnEnter()
        {
            if (m_Owner.controller.searchClassInfo != null)
            {
                m_ClassName = m_Owner.controller.findClassInfo.className;
                ChangeClassName();
                m_Owner.controller.searchClassInfo = null;
            }

        }

        public void OnExit()
        {
            
        }

        public void OnGUI(Rect pos)
        {
            m_ActionType =(ActionType)EditorGUILayout.EnumPopup("Action Type", m_ActionType);

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            YHEditorTools.PushLabelWidth(120);
            var content = new GUIContent("Action Class", "常用名：UnityEngine.Sprite,UnityEngine.Material,Renderer");
            string className = EditorGUILayout.TextField(content, m_ClassName, GUILayout.MinWidth(360));
            bool inherit = EditorGUILayout.Toggle(new GUIContent("Inherit", "show parent field"), m_Inherit);

            if (className != m_ClassName)
            {
                m_ClassName = className;
                m_Inherit = inherit;
                ChangeClassName();
            }

            if (m_Inherit != inherit)
            {
                m_Inherit = inherit;
                ChangeInherit();
            }

            YHEditorTools.PopLabelWidth();
            GUILayout.EndHorizontal();

            if (m_ActionType == ActionType.Express)
            {
                m_ModifyExpressionView.OnGUI(pos);
            }
            else
            {
                EditorGUILayout.LabelField("暂时只支持静态方法");
                int selectIndex = EditorGUILayout.Popup("Methods",m_SelectMethodIndex, m_ClassMethodNames);
                if (m_SelectMethodIndex != selectIndex)
                {
                    m_SelectMethodIndex = selectIndex;
                    UpdateParameters();
                }
                DrawMethodParameters();
            }
            
            if (GUILayout.Button("Modify"))
            {
                DoModify();
            }
        }


        void DrawMethodParameters()
        {
            EditorGUILayout.LabelField("Parameters:");
            foreach(var p in m_Parameters)
            {
               p.Draw();
            }
        }

        void ChangeClassName()
        {
            m_Owner.controller.RefreshModifyClassInfo(m_ClassName, m_Inherit);
            m_ModifyExpressionView.classInfo = m_Owner.controller.modifyClassInfo;

            if (m_Owner.controller.modifyClassInfo != null && m_Owner.controller.modifyClassInfo.type != null)
            {
                GetMethodInfos(m_Owner.controller.modifyClassInfo.type,m_Inherit);
                UpdateParameters();
            }
            else
            {
                m_ClassMethodNames = new string[0];
                m_ClassMethods.Clear();
                m_Parameters.Clear();
            }

            ChangeInherit();
        }

        void ChangeInherit()
        {
            string[] members = m_Owner.controller.modifyClassInfo != null ? m_Owner.controller.modifyClassInfo.GetMemberNames(m_Inherit) : null;
            m_ModifyExpressionView.ChangeExpressionNames(members, true);
        }

        void GetMethodInfos(Type type,bool inhert)
        {
            m_ClassMethods.Clear();

            List<string> names = YH.Pool.ListPool<string>.Get();

            foreach (MethodInfo m in type.GetMethods())
            {
                if (m.IsStatic && (inhert || m.DeclaringType == type) && CheckParameter(m))
                {
                    names.Add(m.ToString());
                    m_ClassMethods.Add(m);
                }
            }
            m_ClassMethodNames= names.ToArray();
            YH.Pool.ListPool<string>.Release(names);
        }

        bool CheckParameter(MethodInfo method)
        {
            ParameterInfo[] parameterInfos = method.GetParameters();
            if (parameterInfos.Length > 0)
            {
                ParameterInfo first = parameterInfos[0];
                if (first.ParameterType.IsArray)
                {
                    return first.ParameterType.GetElementType() == typeof(FindResult);
                }
                else if (first.ParameterType.IsGenericType)
                {
                    return first.ParameterType.GetGenericArguments()[0] == typeof(FindResult);
                }
            }
            return false;
        }

        void UpdateParameters()
        {
            m_Parameters.Clear();
            if (m_ClassMethods.Count > 0)
            {
                MethodInfo methodInfo = m_ClassMethods[m_SelectMethodIndex];
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                
                foreach (var p in parameterInfos)
                {
                    BaseField field = BaseField.Create(null, p.ParameterType, p.Name, 1);
                    m_Parameters.Add(field);
                }
            }
        }

        void DoModify()
        {
            if (m_ActionType == ActionType.Express)
            {
                m_Owner.controller.Modify(m_ModifyExpressionView.GetNotNullExpressions());
            }
            else
            {
                if (m_ClassMethods.Count > 0)
                {
                    MethodInfo selectMethod = m_ClassMethods[m_SelectMethodIndex];
                    List<object> ps = new List<object>();
                    if (selectMethod.GetParameters()[0].ParameterType.IsArray)
                    {
                        ps.Add(m_Owner.controller.findResults.ToArray());
                    }
                    else
                    {
                        ps.Add(m_Owner.controller.findResults);
                    }

                    for (int i = 1; i < m_Parameters.Count; ++i)
                    {
                        ps.Add(m_Parameters[i].value);
                    }
                    selectMethod.Invoke(null, ps.ToArray());
                }
            }
        }
    }
}