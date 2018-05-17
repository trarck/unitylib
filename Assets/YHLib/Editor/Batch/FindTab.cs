using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace YH
{
    [Serializable]
    public class FindClass
    {
        public string className;
        public string[] fieldNames;

        public Type type;
    }

    [Serializable]
    public class FindCondition
    {
        public enum Operation
        {
            Equal,//=
            NotEqual,//!=
            Less,//<
            LessEqual,//<=
            Big,//>
            BigEqual,//>=
            Contains,//contain
        }

        public int propertyIndex = 0;
        public string property;
        public Operation op;
        public string value;
    }

    public class FindResult
    {
        public string path;
        public UnityEngine.Object obj; 
              
        public FindResult(string path,UnityEngine.Object obj)
        {
            this.path = path;
            this.obj = obj;
        }
    }

    [Serializable]
    public class FindTab:IEditorTab
    {
        EditorWindow m_Owner;

        string m_SearchPath= "Assets/Tests/Prefabs/Find";
        string m_Filter= "t:Prefab t:Scene";

        FindClass m_Class = new FindClass();

        List<string> m_FieldNames = new List<string>();
        int m_SelectFieldIndex = 0;

        Vector2 m_ConditionScrollPosition = Vector2.zero;
        List<FindCondition> m_Conditions = new List<FindCondition>();

        List<FindResult> m_Results = new List<FindResult>();
        Vector2 m_ResultScrollPosition = Vector2.zero;

        Dictionary<FindCondition.Operation, RelationalOperator> m_Operators = new Dictionary<FindCondition.Operation, RelationalOperator>();

        public string name { get; set; }
        
        // Use this for initialization
        public void Init(EditorWindow owner)
        {
            m_Class.className = "MyObjA";
            GetClassInfo();

            m_Owner = owner;
        }

        void InitOperators()
        {
            m_Operators[FindCondition.Operation.Equal] = new Equal();
            m_Operators[FindCondition.Operation.NotEqual] = new NotEqual();
            m_Operators[FindCondition.Operation.Less] = new Less();
            m_Operators[FindCondition.Operation.LessEqual] = new LessEqual();
            m_Operators[FindCondition.Operation.Big] = new Big();
            m_Operators[FindCondition.Operation.BigEqual] = new BigEqual();
            m_Operators[FindCondition.Operation.Contains] = new Contains();
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
            string className = EditorGUILayout.TextField("Class Name", m_Class.className);
            if (className!=m_Class.className)
            {
                m_Class.className = className;
                ChangeClassName();
            }
            GUILayout.EndHorizontal();

            DrawConditions();

            if (GUILayout.Button("Search"))
            {
                DoSearch();
            }

            DrawResults();
        }

        void DrawConditions()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Conditions");

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
            if (m_Class.fieldNames != null && m_Class.fieldNames.Length > 0)
            {
                fc.propertyIndex = EditorGUILayout.Popup(fc.propertyIndex, m_Class.fieldNames);
                if (fc.propertyIndex == m_Class.fieldNames.Length - 1)
                {
                    //last one is custom define
                    fc.property = EditorGUILayout.TextField(fc.property);
                }
                else
                {
                    fc.property = m_Class.fieldNames[fc.propertyIndex];
                }
            }
            else
            {
                fc.property = EditorGUILayout.TextField(fc.property);
            }
            
            
            fc.op = (FindCondition.Operation)EditorGUILayout.EnumPopup(fc.op);
            fc.value= EditorGUILayout.TextField(fc.value);
            if (GUILayout.Button("-"))
            {
                RemoveCondition(fc);
            }
            GUILayout.EndHorizontal();
        }

        void DrawResults()
        {
            m_ConditionScrollPosition = EditorGUILayout.BeginScrollView(m_ConditionScrollPosition);

            for (int i = 0; i < m_Results.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(m_Results[i].path);
                m_Results[i].obj= EditorGUILayout.ObjectField(m_Results[i].obj,m_Class.type,false);
                GUILayout.EndHorizontal();
            }


            EditorGUILayout.EndScrollView();
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
            GetClassInfo();
            m_Conditions.Clear();
        }

        void GetClassInfo()
        {
            if (!string.IsNullOrEmpty(m_Class.className))
            {
                m_Class.type = ReflectionUtils.Instance.GetType(m_Class.className);
                if (m_Class.type != null)
                {
                    FieldInfo[] fields = ReflectionUtils.Instance.GetSerializableFields(m_Class.type);
                    string[] names = new string[fields.Length+1];
                    for(int i=0;i<fields.Length;++i)
                    {
                        names[i]= fields[i].Name;
                    }
                    names[fields.Length] = "Custom";
                    m_Class.fieldNames = names;                    
                }
                else
                {
                    m_Class.fieldNames = null;
                }
            }
        }

        void DoSearch()
        {
            if (string.IsNullOrEmpty(m_Class.className))
            {
                return;
            }
           
            FindCondition condition = null;
            object value=null;

            m_Results.Clear();

            List<string> assets = FindAsset.FindAllAssets(m_SearchPath, m_Filter);
            for (int i = 0; i < assets.Count; ++i)
            {
                GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(assets[i]);

                if (gameObj != null)
                {
                    Component[] insts = gameObj.GetComponentsInChildren(m_Class.type);
                    if (insts!=null && insts.Length>0)
                    {
                        for(int j = 0; j < insts.Length; ++j)
                        {

                            if (m_Conditions != null && m_Conditions.Count > 0)
                            {
                                for (int k = 0; k < m_Conditions.Count; ++k)
                                {
                                    condition = m_Conditions[k];
                                    FieldInfo field = m_Class.type.GetField(condition.property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                                    if (field != null)
                                    {
                                        value = field.GetValue(insts[j]);
                                        if (m_Operators.ContainsKey(condition.op) && m_Operators[condition.op].Execute(condition.value, value))
                                        {
                                            m_Results.Add(new FindResult(assets[i] + ":" +HierarchyUtil.FullPath(insts[j].transform), insts[j]));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                m_Results.Add(new FindResult(assets[i] + ":" + HierarchyUtil.FullPath(insts[j].transform), insts[j]));
                            }
                        }
                    }
                }
            }
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