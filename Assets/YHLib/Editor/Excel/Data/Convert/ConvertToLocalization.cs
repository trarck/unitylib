using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace YH.Excel.Data
{
    public class ConvertToLocalization : EditorWindow
    {
        struct LangItem
        {
            public string name;
            public bool convertable;
        }

        IWorkbook m_Workbook;
        Schema m_Schema;

        Vector2 m_ScrollViewPos = Vector2.zero;

        LangItem[] m_LangItems;

        //保存在一个文件，还是多个文件
        bool m_AllInOne = false;
        bool m_BeautifyJson = false;
        string m_KeyField = "";

        [MenuItem("Assets/ExcelData/Convert To Localization")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow<ConvertToLocalization>(false, "Convert To Localization");
        }

        void OnGUI()
        {
            m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load",GUILayout.Width(100)))
            {
                DoLoad();
            }

            if (GUILayout.Button("Convert", GUILayout.Width(100)))
            {
                DoConvert();
            }

            YHEditorTools.PushLabelWidth(70);
            m_AllInOne = EditorGUILayout.Toggle("All In One", m_AllInOne,GUILayout.Width(100));
            YHEditorTools.PopLabelWidth();

            YHEditorTools.PushLabelWidth(60);
            m_BeautifyJson = EditorGUILayout.Toggle("Beautify", m_BeautifyJson, GUILayout.Width(100));
            YHEditorTools.PopLabelWidth();

            YHEditorTools.PushLabelWidth(40);
            m_KeyField = EditorGUILayout.TextField("Key", m_KeyField, GUILayout.Width(100));
            YHEditorTools.PopLabelWidth();

            EditorGUILayout.EndHorizontal();          

            ShowLangGUI();

            EditorGUILayout.EndScrollView();
        }

        void ShowLangGUI()
        {
            if (m_Workbook != null)
            {
                if (!m_AllInOne)
                {
                    for (int i = 0; i < m_LangItems.Length; ++i)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("lang:" + m_LangItems[i].name, GUILayout.Width(100));
                        m_LangItems[i].convertable = EditorGUILayout.Toggle(m_LangItems[i].convertable, GUILayout.Width(50));
                        EditorGUILayout.EndHorizontal();
                    }
                }               
            }
        }

        void DoLoad()
        {
            var xlsFile = EditorUtility.OpenFilePanel("load excel file", "", "xls;*.xlsx;");
            if (xlsFile.Length != 0)
            {
                m_Workbook = ExcelHelper.Load(xlsFile);
                //第一张表
                ISheet sheet = m_Workbook.GetSheetAt(0);

                m_Schema = EDSchemaReader.ReadSchema(sheet);

                //第一列为Key，其它为语言
                List<LangItem> list = new List<LangItem>();

                for(int i = 1; i < m_Schema.fields.Count; ++i)
                {
                    LangItem item = new LangItem();
                    item.name = m_Schema.fields[i].name;
                    item.convertable = true;
                    list.Add(item);
                }

                m_LangItems = list.ToArray();
            }
        }

        void DoConvert()
        {
            if (m_AllInOne)
            {
                var saveFile = EditorUtility.SaveFilePanel("Save lang file", "", "","");
                if (saveFile.Length != 0)
                {
                    ConvertAllInOne(saveFile);
                }
            }
            else
            {
                var savePath = EditorUtility.SaveFolderPanel("Save lang file directory", "", "");
                if (savePath.Length != 0)
                {
                    ConvertSeprate(savePath);
                }
            }
        }

        void ConvertSeprate(string savePath)
        {
            List<object> langData = EDDataReader.ReadList(m_Workbook.GetSheetAt(0), m_Schema);

            string key = string.IsNullOrEmpty(m_KeyField)?  m_Schema.fields[0].name:m_KeyField;

            List<Dictionary<string, object>> multiLangList = new List<Dictionary<string, object>>();
            List<string> enableLangNameList = new List<string>();

            for (int i = 0; i < m_LangItems.Length; ++i)
            {
                if (m_LangItems[i].convertable)
                {
                    multiLangList.Add(new Dictionary<string, object>());
                    enableLangNameList.Add(m_LangItems[i].name);
                }
            }

            //提取数据
            string keyValue;
            foreach (Dictionary<string,object> record in langData)
            {
                for (int i = 0; i < multiLangList.Count; ++i)
                {
                    keyValue = record[key] as string;
                    multiLangList[i][keyValue] = record[enableLangNameList[i]];
                }
            }

            //分别保存
            for (int i = 0; i < multiLangList.Count; ++i)
            {
                string langFile = Path.Combine(savePath, enableLangNameList[i] + ".json");
                SaveToJsonFile(langFile, multiLangList[i]);
            }

            EditorUtility.DisplayDialog("Convert To Lang", "Convert   Lang","OK");
        }

        void ConvertAllInOne(string saveFile)
        {
            Dictionary<string,object> langData = EDDataReader.ReadDictionary(m_Workbook.GetSheetAt(0), m_Schema, m_KeyField);

            //remove key in data field
            string key = string.IsNullOrEmpty(m_KeyField) ? m_Schema.fields[0].name : m_KeyField;
            foreach (KeyValuePair<string,object> iter in langData)
            {
                Dictionary<string, object> record = iter.Value as Dictionary<string, object>;
                record.Remove(key);
            }

            SaveToJsonFile(saveFile, langData);
            EditorUtility.DisplayDialog("Convert To Lang", "Convert   Lang", "OK");
        }

        void SaveToJsonFile(string jsonfile, object data)
        {
            fastJSON.JSONParameters param = new fastJSON.JSONParameters();
            param.UseEscapedUnicode = false;

            string jsonString = fastJSON.JSON.ToJSON(data, param);
            if (m_BeautifyJson)
            {
                jsonString = fastJSON.JSON.Beautify(jsonString);
            }           
            
            File.WriteAllText(jsonfile, jsonString);
        }
    }
}