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

        Vector2 m_ConditionScrollPosition = Vector2.zero;

        string[] m_ConditionNames;

        List<FindCondition> m_Conditions = new List<FindCondition>();

        public string name { get; set; }
        
        // Use this for initialization
        public void Init(EditorTabs owner)
        {
            m_Owner = (BatchMain)owner;
            m_ClassName = "UnityEngine.SpriteRenderer";
            m_Owner.controller.findClassInfo = Batch.GetClassInfo(m_ClassName,m_Inherit);
            m_ConditionNames = m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit);
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

            if (m_ConditionNames != null && m_ConditionNames.Length > 0)
            {
                int index = EditorGUILayout.Popup(fc.index, m_ConditionNames);

                if (index == m_ConditionNames.Length - 1)
                {
                    fc.index = index;
                    //last one is custom define
                    string name = EditorGUILayout.TextField(fc.name);
                    if (fc.name != name)
                    {
                        ChangeCondition(fc, index, name);
                    }
                }
                else if(fc.index!=index)
                {
                    ChangeCondition(fc, index, name);
                }
            }
            else
            {
                string name = EditorGUILayout.TextField(fc.name);
                if (fc.name != name)
                {
                    ChangeCondition(fc, 1, name);
                }
            }
            
            fc.op = (FindCondition.Operation)EditorGUILayout.EnumPopup(fc.op);

            DrawConditionValue(fc);

            if (GUILayout.Button("-"))
            {
                RemoveCondition(fc);
            }
            GUILayout.EndHorizontal();
        }

        void DrawConditionValue(FindCondition fc)
        {
            if (fc.type != null)
            {
                switch (fc.type.ToString())
                {
                    case "System.Int32":
                        fc.value = EditorGUILayout.IntField(fc.value!=null?(int)fc.value:0);
                        break;
                    case "System.Int64":
                        fc.value = EditorGUILayout.LongField(fc.value != null?(long)fc.value:0);
                        break;
                    case "System.Single":
                        fc.value = EditorGUILayout.FloatField(fc.value != null ? (float)fc.value:0);
                        break;
                    case "System.Double":
                        fc.value = EditorGUILayout.DoubleField(fc.value != null ? (double)fc.value:0);
                        break;

                    case "UnityEngine.Vect2":
                        fc.value = EditorGUILayout.Vector2Field("", fc.value != null ? (Vector2)fc.value:Vector2.zero);
                        break;
                    case "UnityEngine.Vect3":
                        fc.value = EditorGUILayout.Vector3Field("", fc.value != null ? (Vector3)fc.value:Vector3.zero);
                        break;
                    case "UnityEngine.Vect4":
                        fc.value = EditorGUILayout.Vector4Field("", fc.value != null ? (Vector4)fc.value:Vector4.zero);
                        break;
                    case "UnityEngine.Rect":
                        fc.value = EditorGUILayout.RectField(fc.value != null ? (Rect)fc.value:Rect.zero);
                        break;
                    case "UnityEngine.Bounds":
                        fc.value = EditorGUILayout.BoundsField((Bounds)fc.value);
                        break;
                    case "UnityEngine.Color":
                        fc.value = EditorGUILayout.ColorField((Color)fc.value);
                        break;

                    case "UnityEngine.AnimationCurve":
                        fc.value = EditorGUILayout.CurveField((AnimationCurve)fc.value);
                        break;

                    default:

                        if (fc.type.IsSubclassOf(typeof(UnityEngine.Object)))
                        {
                            fc.value = EditorGUILayout.ObjectField((UnityEngine.Object)fc.value, fc.type, false);
                        }
                        else
                        {
                            fc.value = EditorGUILayout.TextField(fc.value!=null?fc.value.ToString():"");
                        }
                        break;
                }
            }
        }

        void AddCondition()
        {
            FindCondition fc = new FindCondition();
            ChangeCondition(fc,m_Conditions.Count,"");
            m_Conditions.Add(fc);
        }

        void ChangeCondition(FindCondition fc,int index,string name=null)
        {
            if (m_ConditionNames != null)
            {
                if (index < m_ConditionNames.Length - 1)
                {
                    fc.index = index;
                    fc.name = m_ConditionNames[fc.index];
                }
                else
                {
                    fc.index = m_ConditionNames.Length - 1;
                    fc.name = name;
                }
            }
            else
            {
                fc.index = 0;
                fc.name = name;
            }

            Type newType = m_Owner.controller.findClassInfo.GetMemberType(fc.name);
            if (newType != fc.type)
            {
                fc.type = newType;
                fc.value = null;
            }
        }

        void RemoveCondition(FindCondition fc)
        {
            m_Conditions.Remove(fc);
        }

        void ChangeClassName()
        {
            m_Owner.controller.RefreshFindClassInfo(m_ClassName,m_Inherit);
            m_ConditionNames = m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit);
            m_Conditions.Clear();
        }

        void ChangeInherit()
        {
            m_ConditionNames = m_Owner.controller.findClassInfo.GetMemberNames(m_Inherit);

            List<FindCondition> keeps = new List<FindCondition>();

            for(int i = 0; i < m_Conditions.Count; ++i)
            {
                for(int j=0;j< m_ConditionNames.Length; ++j)
                {
                    if (m_Conditions[i].name == m_ConditionNames[j])
                    {
                        m_Conditions[i].index = j;
                        keeps.Add(m_Conditions[i]);
                        break;
                    }
                }
            }

            m_Conditions = keeps;
        }

        List<FindCondition> GetNotNullConditions()
        {
            List<FindCondition> keeps = new List<FindCondition>();
            for (int i = 0; i < m_Conditions.Count; ++i)
            {
                if (m_Conditions[i].value!=null)
                {
                    keeps.Add(m_Conditions[i]);
                }
            }
            return keeps;
        }

        void DoSearch()
        {
            if (string.IsNullOrEmpty(m_ClassName))
            {
                return;
            }

            Debug.Log(typeof(int).ToString());
            Debug.Log(typeof(long).ToString());
            Debug.Log(typeof(float).ToString());
            Debug.Log(typeof(double).ToString());
            Debug.Log(typeof(SpriteRenderer).ToString());

            m_Owner.controller.findResults = m_Owner.controller.Search(m_SearchPath, m_Filter, m_Owner.controller.findClassInfo, GetNotNullConditions());
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