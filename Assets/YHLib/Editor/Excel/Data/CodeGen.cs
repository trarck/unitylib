using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YH.Excel.Data
{
    public class CodeGen
    {
        public void GenClass(Schema schema)
        {
            string template = System.IO.File.ReadAllText(Application.dataPath + "/YHLib/Editor/Excel/Data/CodeDataTemplate.ts");
            template=template.Replace("{PROPERTIES}", CreateProperties(schema));
            Debug.Log(template);
        }

        string GetFieldTypeDefine(Field field)
        {
            string typeDefine = "object";

            switch (field.type)
            {
                case ExcelDataType.String:
                    typeDefine = "string";
                    break;
                case ExcelDataType.Int:
                    typeDefine = "int";
                    break;
                case ExcelDataType.Float:
                    typeDefine = "float";
                    break;
                case ExcelDataType.Array:
                    typeDefine = (field.extType==""?"string":field.extType)+"[]";
                    break;
                case ExcelDataType.List:
                    typeDefine = "List<"+ (field.extType == "" ? "object" : field.extType) + ">";
                    break;
                case ExcelDataType.Dictionary:                    
                    typeDefine = "Dictionary<"+ (field.extType == "" ? "string,object" : field.extType) + ">";
                    break;
                case ExcelDataType.Long:
                    typeDefine = "long";
                    break;
                case ExcelDataType.Double:
                    typeDefine = "double";
                    break;
                case ExcelDataType.Custom:
                    typeDefine = field.extType;
                    break;
            }

            return typeDefine;
        }

        string CreateProperty(Field field)
        {
            return "public " + GetFieldTypeDefine(field) + " " + field.name+";\n";
        }

        string CreateProperties(Schema shema)
        {
            string properties = "";
            foreach(Field field in shema.fields)
            {
                properties += CreateProperty(field);
            }
            return properties;
        }
    }
}