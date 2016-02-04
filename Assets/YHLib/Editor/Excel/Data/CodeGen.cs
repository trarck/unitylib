using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace YH.Excel.Data
{
    public class CodeGen
    {
        static int PropertyPad = 4;
        static string CRLF = "\r\n";
        string m_Ns;

        public void GenClass(Schema schema,string outputPath)
        {
            string template = System.IO.File.ReadAllText(Application.dataPath + "/YHLib/Editor/Excel/Data/CodeDataTemplate.ts");
            template = template.Replace("{CLASS}", schema.name);
            template=template.Replace("{PROPERTIES}", CreateProperties(schema));
            if (string.IsNullOrEmpty(m_Ns))
            {
                template = template.Replace("{NAMESPACE_START}","");
                template = template.Replace("{NAMESPACE_END}", "");
            }
            else
            {
                template = template.Replace("{NAMESPACE_START}", "namespace "+m_Ns+"\n{");
                template = template.Replace("{NAMESPACE_END}", "}");
            }

            //check path is exists
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            string codeFile = Path.Combine(outputPath , schema.name + ".cs");
            File.WriteAllText(codeFile, template);
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
            string pad = Pad(PropertyPad);

            string comment = "";
            if (field.comment != "")
            {
                comment = pad + "/*" + field.comment + "*/"+ CRLF;
            }
            return comment
                + pad+"public " + GetFieldTypeDefine(field) + " " + field.name+";"+ CRLF;
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

        public static string Pad(int num)
        {
            string p = "";
            for(int i = 0; i < num; ++i)
            {
                p += " ";
            }

            return p;
        }

        public string ns
        {
            set
            {
                m_Ns = value;
            }

            get
            {
                return m_Ns;
            }
        }
    }
}