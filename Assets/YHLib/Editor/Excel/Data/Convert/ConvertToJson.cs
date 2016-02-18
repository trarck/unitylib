using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace YH.Excel.Data
{
    public class ConvertToJson : EditorWindow
    {
        enum ConvertSheetDataType
        {
            List,
            Dictionary
        }

        struct SheetConvertItem
        {
            public string name;
            public bool convertable;
            public ConvertSheetDataType dataType;
            public string dictKey;
        }

        IWorkbook m_Workbook;

        Vector2 m_ScrollViewPos = Vector2.zero;

        SheetConvertItem[] m_SheetConverts;

        bool m_BeautifyJson = false;

        [MenuItem("Assets/ExcelData/Convert To Json")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow<ConvertToJson>(false,"Convert To Json");
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

            YHEditorTools.PushLabelWidth(40);
            m_BeautifyJson = EditorGUILayout.Toggle("Beautify", m_BeautifyJson);
            YHEditorTools.PopLabelWidth();

            EditorGUILayout.EndHorizontal();

            ShowSheetGUI();

            EditorGUILayout.EndScrollView();
        }

        void ShowSheetGUI()
        {
            if (m_Workbook != null)
            {
                for (int i = 0; i < m_SheetConverts.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField( "table:"+m_SheetConverts[i].name,GUILayout.Width(100));
                    m_SheetConverts[i].convertable = EditorGUILayout.Toggle(m_SheetConverts[i].convertable, GUILayout.Width(50));

                    float oldWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth= 40;
                    m_SheetConverts[i].dataType=(ConvertSheetDataType)EditorGUILayout.EnumPopup("type", m_SheetConverts[i].dataType,GUILayout.MaxWidth(180));

                    if (m_SheetConverts[i].dataType == ConvertSheetDataType.Dictionary)
                    {
                        m_SheetConverts[i].dictKey=EditorGUILayout.TextField("key",m_SheetConverts[i].dictKey);
                    }

                    EditorGUIUtility.labelWidth = oldWidth;

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

                List < SheetConvertItem > list = new List<SheetConvertItem>();

                for (int i = 0; i < m_Workbook.NumberOfSheets; ++i)
                {
                    ISheet sheet = m_Workbook.GetSheetAt(i);
                    if (IsTableSheet(sheet))
                    {
                        SheetConvertItem item = new SheetConvertItem();
                        item.name = sheet.SheetName;
                        item.convertable = true;
                        item.dataType = ConvertSheetDataType.List;
                        item.dictKey = "";

                        list.Add(item);
                    }
                }
                m_SheetConverts = list.ToArray();
            }
        }

        void DoConvert()
        {
            var savePath = EditorUtility.SaveFolderPanel("Save json file directory", "", "");
            if (savePath.Length != 0)
            {
                ConvertWorkbook(savePath);
            }
        }

        void ConvertWorkbook(string savePath)
        {
            int n = 0;
            for (int i = 0; i < m_SheetConverts.Length; ++i)
            {
                if (m_SheetConverts[i].convertable)
                {
                    ISheet sheet = m_Workbook.GetSheet(m_SheetConverts[i].name);
                    if (sheet != null)
                    {
                        ++n;
                        if (m_SheetConverts[i].dataType == ConvertSheetDataType.List)
                        {
                            ConvertSheet(sheet, savePath);
                        }
                        else if (m_SheetConverts[i].dataType == ConvertSheetDataType.Dictionary)
                        {
                            ConvertSheet(sheet, savePath,m_SheetConverts[i].dictKey);
                        }
                        EditorUtility.DisplayProgressBar("Convert Sheet", n + "/" + m_SheetConverts.Length, 1.0f*n / m_SheetConverts.Length);                   
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Convert To Json", "Convert " + n + " Sheets","ok");
        }

        void ConvertSheet(ISheet sheet,string savePath)
        {
            Schema schema = EDSchemaReader.ReadSchema(sheet);
            EDDataReader reader = new EDDataReader();
            object list = EDDataReader.ReadList(sheet, schema);

            string filename = Path.Combine(savePath, schema.name + ".json");
            SaveToJsonFile(filename, list);
        }

        void ConvertSheet(ISheet sheet, string savePath,string keyName)
        {
            Schema schema = EDSchemaReader.ReadSchema(sheet);
            EDDataReader reader = new EDDataReader();
            object dict = EDDataReader.ReadDictionary(sheet, schema,keyName);

            string filename = Path.Combine(savePath, schema.name + ".json");
            SaveToJsonFile(filename, dict);
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

        bool IsTableSheet(ISheet sheet)
        {
            return sheet.SheetName.IndexOf("__") != 0;
        }
    }
}