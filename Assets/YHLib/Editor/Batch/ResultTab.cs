using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using YH;

namespace YHEditor
{
    public class ResultTab:IEditorTab
    {
        BatchMain m_Owner;

        Vector2 m_ResultScrollPosition = Vector2.zero;

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

        }

        public void OnExit()
        {

        }

        public void OnGUI(Rect pos)
        {
            EditorGUILayout.Space();
            DrawResults();
        }

        void DrawResults()
        {
            List<FindResult> results = m_Owner.controller.findResults;

            if (results != null && results.Count > 0)
            {
                m_ResultScrollPosition = EditorGUILayout.BeginScrollView(m_ResultScrollPosition);

                for (int i = 0; i < results.Count; ++i)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(results[i].path);
                    if (m_Owner.controller.findClassInfo != null)
                    {
                        results[i].obj = EditorGUILayout.ObjectField(results[i].obj, m_Owner.controller.findClassInfo.type, false);
                    }
                    else
                    {
                        results[i].obj = EditorGUILayout.ObjectField(results[i].obj, typeof(UnityEngine.Object), false);
                    }
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }
        }
    }
}