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

        ModifyExpressionView m_ModifyExpressionView;

        public string name { get; set; }
        
        // Use this for initialization
        public void Init(EditorTabs owner)
        {
            m_Owner = (BatchMain)owner;
            m_ModifyExpressionView = new ModifyExpressionView();
            m_ModifyExpressionView.Init(m_Owner.controller);
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
            m_ModifyExpressionView.ChangeExpressionNames(m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit), true);
        }

        public void OnExit()
        {

        }

        public void OnGUI(Rect pos)
        {
            EditorGUILayout.Space();

            m_ModifyExpressionView.OnGUI(pos);

            if (GUILayout.Button("Modify"))
            {
                DoModify();
            }
        }

        
        void ChangeInherit()
        {
            m_ModifyExpressionView.ChangeExpressionNames(m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit), true);
        }

        void DoModify()
        {
            m_Owner.controller.Modify(m_ModifyExpressionView.GetNotNullExpressions());
        }
    }
}