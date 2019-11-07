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
        //查找类型
        public enum SearchType
        {
            //查找组件
            Component,
            //查找引用
            Refrence,
            //查找资源
            Asset
        }

        //查找方法
        public enum SearchMethod
        {
            //Unity资源库
            AssetDatabase,
            //文件系统
            FileSystem,
        }

        BatchMain m_Owner;

        string m_SearchPath= "Assets/Tests/Prefabs/Find";
        SearchType m_SearchType = SearchType.Component;
        SearchMethod m_SearchMethod = SearchMethod.AssetDatabase;

        string m_Filter = "\\.fbx$";

        string m_ClassName = "UnityEditor.ModelImporter";

        bool m_Inherit=false;

        UnityEngine.Object m_FindObject;

        FindConditionView m_FindConditionView;

        public string name { get; set; }

        // Use this for initialization
        public void Init(EditorTabs owner)
        {
            m_Owner = (BatchMain)owner;
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

            //查找目录
            GUILayout.BeginHorizontal();
            YHEditorTools.PushLabelWidth(80);
            m_SearchPath = EditorGUILayout.TextField("Search Path", m_SearchPath);
            YHEditorTools.PopLabelWidth();

            if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f)))
                BrowseForFolder();
            if (GUILayout.Button("Reset", GUILayout.MaxWidth(75f)))
                ResetPathToDefault();
            //if (string.IsNullOrEmpty(m_OutputPath))
            //    m_OutputPath = EditorUserBuildSettings.GetPlatformSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), "AssetBundleOutputPath");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //查找类型
            m_SearchType = (SearchType)EditorGUILayout.EnumPopup("Search Type", m_SearchType, GUILayout.Width(300));
            EditorGUILayout.Space();
            //查找方式
            m_SearchMethod = (SearchMethod)EditorGUILayout.EnumPopup("Search Method", m_SearchMethod, GUILayout.Width(300));

            //过虑器
            var content = new GUIContent("Filter", "根据SerchMethod不同 filter不同.\nUnity资源库则使用Unity的规则:\nType:预设 t:Prefab;场景 t:Scene;模型 t:Mesh 材质:t:Material;图片:Texture.\n文件系统使用正则表达式。");
            m_Filter = EditorGUILayout.TextField(content, m_Filter);
            EditorGUILayout.Space();

            if (m_SearchType == SearchType.Component || m_SearchType==SearchType.Asset)
            {

                GUILayout.BeginHorizontal();
                YHEditorTools.PushLabelWidth(120);
                string labelName = m_SearchType == SearchType.Component ? "Component Name" : "Asset Type";
                content = new GUIContent(labelName, "常用名：UnityEngine.Sprite,UnityEngine.Material,Renderer");
                string className = EditorGUILayout.TextField(content, m_ClassName, GUILayout.MinWidth(360));
                bool inherit = EditorGUILayout.Toggle(new GUIContent("Inherit", "show parent field"),m_Inherit);

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
                //查找引用
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
            string[] members = m_Owner.controller.findClassInfo != null ? m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit):null;
            m_FindConditionView.ChangeExpressionNames(members, false);
        }

        void ChangeInherit()
        {
            string[] members = m_Owner.controller.findClassInfo != null ? m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit) : null;
            m_FindConditionView.ChangeExpressionNames(members, true);
        }

        void DoSearch()
        {
            switch (m_SearchType)
            {
                case SearchType.Component:
                    if ( m_Owner.controller.findClassInfo == null || m_Owner.controller.findClassInfo.type == null)
                    {
                        return;
                    }

                    m_Owner.controller.findResults = m_Owner.controller.FindComponents(m_SearchPath, m_Filter, m_SearchMethod == SearchMethod.FileSystem, m_Owner.controller.findClassInfo, m_FindConditionView.root);
                    break;
                case SearchType.Refrence:
                    if (m_FindObject != null)
                    {
                        m_Owner.controller.findResults = m_Owner.controller.FindRefrences(m_SearchPath, m_Filter, m_SearchMethod == SearchMethod.FileSystem, AssetDatabase.GetAssetPath(m_FindObject));
                    }
                    break;
                case SearchType.Asset:
                    m_Owner.controller.findResults = m_Owner.controller.FindAssets(m_SearchPath, m_Filter, m_SearchMethod==SearchMethod.FileSystem, m_Owner.controller.findClassInfo, m_FindConditionView.root);
                    break;
            }
            m_Owner.controller.searchClassInfo = m_Owner.controller.findClassInfo;
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