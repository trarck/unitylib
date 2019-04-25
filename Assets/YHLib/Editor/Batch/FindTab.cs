using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using YH;

namespace YHEditor
{
    
    [Serializable]
    public class FindTab:IEditorTab
    {
        public enum SearchType
        {
            //查找内容
            Content,
            //查找引用
            Refrence,
            //查找自己
            Resource
        }

        BatchMain m_Owner;

        string m_SearchPath= "Assets/Tests/Prefabs/Find";
        SearchType m_SearchType = SearchType.Content;
        string m_Filter = "t:Prefab t:Scene";

        string m_ClassName ="";

        bool m_Inherit=false;

        UnityEngine.Object m_FindObject;

        FindConditionView m_FindConditionView;

        public string name { get; set; }

        // Use this for initialization
        public void Init(EditorTabs owner)
        {
            m_Owner = (BatchMain)owner;
            m_ClassName = "UnityEngine.SpriteRenderer";
            m_Owner.controller.findClassInfo = Batch.GetClassInfo(m_ClassName,m_Inherit);

            m_FindConditionView = new FindConditionView();
            m_FindConditionView.Init(m_Owner.controller.findClassInfo);
            if (m_Owner.controller.findClassInfo != null)
                m_FindConditionView.expressionNames = m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit);
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
            GUILayout.BeginHorizontal();
            YHEditorTools.PushLabelWidth(80);
            var newPath = EditorGUILayout.TextField("Search Path", m_SearchPath);
            YHEditorTools.PopLabelWidth();
            if ((newPath != m_SearchPath) && (newPath != string.Empty))
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

            m_SearchType = (SearchType)EditorGUILayout.EnumPopup("Search Type", m_SearchType, GUILayout.Width(300));
            EditorGUILayout.Space();

            m_Filter = EditorGUILayout.TextField("Filter", m_Filter);
            EditorGUILayout.Space();

            if (m_SearchType == SearchType.Content || m_SearchType==SearchType.Resource)
            {

                GUILayout.BeginHorizontal();
                YHEditorTools.PushLabelWidth(80);
                string className = EditorGUILayout.TextField("Class Name", m_ClassName, GUILayout.MinWidth(360));
                bool inherit = EditorGUILayout.Toggle(m_Inherit);
                EditorGUILayout.LabelField("Inherit");

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

                m_FindConditionView.OnGUI(pos);
            }
            else
            {
                m_FindObject = EditorGUILayout.ObjectField("Object", m_FindObject, typeof(UnityEngine.Object), false);
            }

            if (GUILayout.Button("Search"))
            {
                DoSearch();
            }
        }

        void ChangeClassName()
        {
            m_Owner.controller.RefreshFindClassInfo(m_ClassName,m_Inherit);
            m_FindConditionView.classInfo = m_Owner.controller.findClassInfo;
            m_FindConditionView.ChangeExpressionNames(m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit), false);
        }

        void ChangeInherit()
        {
            m_FindConditionView.ChangeExpressionNames(m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit), true);
        }

        void DoSearch()
        {
            switch (m_SearchType)
            {
                case SearchType.Content:
                    if ( m_Owner.controller.findClassInfo == null || m_Owner.controller.findClassInfo.type == null)
                    {
                        return;
                    }

                    m_Owner.controller.findResults = m_Owner.controller.FindComponents(m_SearchPath, m_Filter, m_Owner.controller.findClassInfo, m_FindConditionView.root);
                    break;
                case SearchType.Refrence:
                    if (m_FindObject != null)
                    {
                        m_Owner.controller.findResults = m_Owner.controller.FindRefrences(m_SearchPath, m_Filter, AssetDatabase.GetAssetPath(m_FindObject));
                    }
                    break;
                case SearchType.Resource:
                    if (m_Owner.controller.findClassInfo == null || m_Owner.controller.findClassInfo.type == null)
                    {
                        return;
                    }

                    m_Owner.controller.findResults = m_Owner.controller.FindResources(m_SearchPath, m_Filter, m_Owner.controller.findClassInfo, m_FindConditionView.root);
                    break;
            }
            
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