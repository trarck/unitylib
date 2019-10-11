using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using YHEditor;

namespace YH.Excel.Data
{
    public class CodeGenWindow : EditorWindow
    {
        struct SheetGenItem
        {
            public string name;
            public bool convertable;
        }

        IWorkbook m_Workbook;

        Vector2 m_ScrollViewPos = Vector2.zero;

        SheetGenItem[] m_SheetGens;

        string m_GenNamespace = "";
        bool m_BeautifyJson = false;

        [MenuItem("Assets/ExcelData/CodeGen")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow<CodeGenWindow>(false, "CodeGen");
        }

        void OnGUI()
        {
            m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load", GUILayout.Width(100)))
            {
                DoLoad();
            }

            if (GUILayout.Button("Convert", GUILayout.Width(100)))
            {
                DoConvert();
            }

            YHEditorTools.PushLabelWidth(30);
            m_GenNamespace = EditorGUILayout.TextField("ns", m_GenNamespace, GUILayout.Width(200));
            YHEditorTools.PopLabelWidth();

            YHEditorTools.PushLabelWidth(60);
            m_BeautifyJson = EditorGUILayout.Toggle("Beautify", m_BeautifyJson, GUILayout.Width(100));
            YHEditorTools.PopLabelWidth();

            EditorGUILayout.EndHorizontal();

            ShowSheetGUI();

            EditorGUILayout.EndScrollView();
        }

        void ShowSheetGUI()
        {
            if (m_Workbook != null)
            {
                for (int i = 0; i < m_SheetGens.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();

                    YHEditorTools.PushLabelWidth(40);
                    m_SheetGens[i].name=EditorGUILayout.TextField("Class:" ,m_SheetGens[i].name, GUILayout.Width(200));
                    YHEditorTools.PopLabelWidth();
                    m_SheetGens[i].convertable = EditorGUILayout.Toggle(m_SheetGens[i].convertable, GUILayout.Width(50));

                    EditorGUILayout.EndHorizontal();
                }
            }
        }


        void DoLoad()
        {
            var xlsFile = EditorUtility.OpenFilePanel("load excel file", "", "xls;*.xlsx;");
            if (xlsFile.Length != 0)
            {
                m_Workbook = ExcelHelper.Load(xlsFile);

                List<SheetGenItem> list = new List<SheetGenItem>();

                for (int i = 0; i < m_Workbook.NumberOfSheets; ++i)
                {
                    ISheet sheet = m_Workbook.GetSheetAt(i);
                    if (IsTableSheet(sheet))
                    {
                        SheetGenItem item = new SheetGenItem();
                        item.name = sheet.SheetName;
                        item.convertable = true;

                        list.Add(item);
                    }
                }
                m_SheetGens = list.ToArray();
            }
        }

        void DoConvert()
        {
            var savePath = EditorUtility.SaveFolderPanel("Save code file directory", "", "");
            if (savePath.Length != 0)
            {
                ConvertWorkbook(savePath);
            }
        }

        void ConvertWorkbook(string savePath)
        {
            int n = 0;
            for (int i = 0; i < m_SheetGens.Length; ++i)
            {
                if (m_SheetGens[i].convertable)
                {
                    ISheet sheet = m_Workbook.GetSheet(m_SheetGens[i].name);
                    if (sheet != null)
                    {
                        ++n;
                        ConvertSheet(sheet, savePath,m_SheetGens[i].name);                        
                    }
                }

                EditorUtility.DisplayProgressBar("GenCode", n + "/" + m_SheetGens.Length, 1.0f * n / m_SheetGens.Length);
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("GenCode", "Gen " + n + " Sheets", "ok");
        }

        void ConvertSheet(ISheet sheet, string savePath,string schemaName)
        {
            Schema schema = EDSchemaReader.ReadSchema(sheet);
            schema.name = schemaName;

            CodeGen gen = new CodeGen();

            gen.ns =m_GenNamespace;
            gen.GenClass(schema, savePath);
        }        

        bool IsTableSheet(ISheet sheet)
        {
            return sheet.SheetName.IndexOf("__") != 0;
        }
    }
}