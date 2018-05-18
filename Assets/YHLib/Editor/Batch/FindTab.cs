using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace YH
{
    
    [Serializable]
    public class FindTab:IEditorTab
    {
        BatchMain m_Owner;

        string m_SearchPath= "Assets/Tests/Prefabs/Find";
        string m_Filter= "t:Prefab t:Scene";
        string m_ClassName ="";

        int m_SelectFieldIndex = 0;

        Vector2 m_ConditionScrollPosition = Vector2.zero;
        List<FindCondition> m_Conditions = new List<FindCondition>();

        public string name { get; set; }
        
        // Use this for initialization
        public void Init(EditorTabs owner)
        {
            m_Owner = (BatchMain)owner;
            m_ClassName = "MyObjA";
            m_Owner.controller.findClassInfo = Batch.GetClassInfo(m_ClassName);

        }

        // Update is called once per frame
        public void Update(float delta)
        {
            
        }

        public void OnGUI(Rect pos)
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            YHEditorTools.PushLabelWidth(80);
            var newPath = EditorGUILayout.TextField("Search Path", m_SearchPath);
            YHEditorTools.PopLabelWidth();
            if ((newPath != m_SearchPath) &&(newPath != string.Empty))
            {
                m_SearchPath = newPath;
            }

            if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f)))
                BrowseForFolder();
            if (GUILayout.Button("Reset", GUILayout.MaxWidth(75f)))
                ResetPathToDefault();
            //if (string.IsNullOrEmpty(m_OutputPath))
            //    m_OutputPath = EditorUserBuildSettings.GetPlatformSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), "AssetBundleOutputPath");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            m_Filter= EditorGUILayout.TextField("Filter", m_Filter);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            string className = EditorGUILayout.TextField("Class Name", m_ClassName);
            if (className!= m_ClassName)
            {
                m_ClassName = className;
                ChangeClassName();
            }
            GUILayout.EndHorizontal();

            DrawConditions();

            if (GUILayout.Button("Search"))
            {
                DoSearch();
            }
        }

        void DrawConditions()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Conditions");

            if (GUILayout.Button("+"))
            {
                AddCondition();
            }

            GUILayout.EndHorizontal();

            if (m_Conditions != null && m_Conditions.Count > 0)
            {
                m_ConditionScrollPosition = EditorGUILayout.BeginScrollView(m_ConditionScrollPosition);

                for (int i = 0; i < m_Conditions.Count; ++i)
                {
                    DrawCondition(m_Conditions[i]);
                }

                EditorGUILayout.EndScrollView();
            }
        }


        void DrawCondition(FindCondition fc)
        {
            GUILayout.BeginHorizontal();
            ClassInfo findClassInfo = m_Owner.controller.findClassInfo;

            if (findClassInfo.fieldNames != null && findClassInfo.fieldNames.Length > 0)
            {
                fc.fieldIndex = EditorGUILayout.Popup(fc.fieldIndex, findClassInfo.fieldNames);
                if (fc.fieldIndex == findClassInfo.fieldNames.Length - 1)
                {
                    //last one is custom define
                    fc.field = EditorGUILayout.TextField(fc.field);
                }
                else
                {
                    fc.field = findClassInfo.fieldNames[fc.fieldIndex];
                }
            }
            else
            {
                fc.field = EditorGUILayout.TextField(fc.field);
            }
            
            
            fc.op = (FindCondition.Operation)EditorGUILayout.EnumPopup(fc.op);
            fc.value= EditorGUILayout.TextField(fc.value);
            if (GUILayout.Button("-"))
            {
                RemoveCondition(fc);
            }
            GUILayout.EndHorizontal();
        }

        void AddCondition()
        {
            FindCondition fc = new FindCondition();
            m_Conditions.Add(fc);
        }

        void RemoveCondition(FindCondition fc)
        {
            m_Conditions.Remove(fc);
        }

        void ChangeClassName()
        {
            m_Owner.controller.RefreshFindClassInfo(m_ClassName);

            m_Conditions.Clear();
        }
        
        void DoSearch()
        {
            if (string.IsNullOrEmpty(m_ClassName))
            {
                return;
            }
            m_Owner.controller.findResults = m_Owner.controller.Search(m_SearchPath, m_Filter, m_Owner.controller.findClassInfo, m_Conditions);
            m_Owner.ChangeTab("Result");
        }

        private void BrowseForFolder()
        {
            var newPath = EditorUtility.OpenFolderPanel("Search Path", m_SearchPath, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                var gamePath = System.IO.Path.GetFullPath(".");
                gamePath = gamePath.Replace("\\", "/");
                if (newPath.StartsWith(gamePath) && newPath.Length > gamePath.Length)
                    newPath = newPath.Remove(0, gamePath.Length + 1);
                m_SearchPath = newPath;
            }
        }

        private void ResetPathToDefault()
        {
            m_SearchPath = "Assets";
        }
    }
}