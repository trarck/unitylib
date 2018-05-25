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
        bool m_Inherit=false;

        FindConditionView m_FindConditionView;

        public string name { get; set; }

        object myValue;
        // Use this for initialization
        public void Init(EditorTabs owner)
        {
            m_Owner = (BatchMain)owner;
            m_ClassName = "UnityEngine.SpriteRenderer";
            m_Owner.controller.findClassInfo = Batch.GetClassInfo(m_ClassName,m_Inherit);

            m_FindConditionView = new FindConditionView();
            m_FindConditionView.Init(m_Owner.controller);
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
            YHEditorTools.PushLabelWidth(80);
            string className = EditorGUILayout.TextField("Class Name", m_ClassName,GUILayout.MinWidth(360));
            bool inherit = EditorGUILayout.Toggle(m_Inherit);
            EditorGUILayout.LabelField("Inherit");

            if (className!= m_ClassName)
            {
                m_ClassName = className;
                m_Inherit = inherit;
                ChangeClassName();
            }

            if(m_Inherit != inherit)
            {
                m_Inherit = inherit;
                ChangeInherit();
            }


            YHEditorTools.PopLabelWidth();
            GUILayout.EndHorizontal();

            m_FindConditionView.OnGUI(pos);

            if (GUILayout.Button("Search"))
            {
                DoSearch();
            }

            mma = (string[])YHEditor.YHGUI.DrawArray(mma, mma.GetType(), ref fodout, "test");
        }

        string[] mma = new string[] { "1", "3", "2" };

        void ChangeClassName()
        {
            m_Owner.controller.RefreshFindClassInfo(m_ClassName,m_Inherit);
            m_FindConditionView.ChangeExpressionNames(m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit), false);
        }

        void ChangeInherit()
        {
            m_FindConditionView.ChangeExpressionNames(m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit), true);
        }

        bool fodout = false;
        void DoSearch()
        {
            if (string.IsNullOrEmpty(m_ClassName))
            {
                return;
            }

            //string[] a = new string[] { "1","3","2"};

            //MemberInfo[] ms = a.GetType().GetMembers();

            //object aa = a.GetType().InvokeMember("Set", BindingFlags.CreateInstance, null, a, new object[] { 6 });

            //MethodInfo m = a.GetType().GetMethod("GetValue",new Type[] { typeof(int)});

            //object[] param = new object[] { 1 };

            //object o=m.Invoke(a, param);

            //fodout=YHEditor.YHGUI.DrawArray(a, a.GetType(), fodout, "test");
          //  m_Owner.controller.findResults = m_Owner.controller.Search(m_SearchPath, m_Filter, m_Owner.controller.findClassInfo, m_FindConditionView.GetNotNullExpressions());
          // m_Owner.ChangeTab("Result");
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