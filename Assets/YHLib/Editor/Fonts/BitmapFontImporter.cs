using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using System.Xml;

using YH.Fonts;

public static class BitmapFontImporter {
 
    [MenuItem("Assets/BitmapFont/Import Font")]
    public static void GenerateFont()
    {
        TextAsset selected = (TextAsset)Selection.activeObject;

        if (!selected) throw new UnityException(selected.name + "is not a valid font-xml file");
        string fontFile = AssetDatabase.GetAssetPath(selected);
        string rootPath = Path.GetDirectoryName(fontFile);
        
        BitmapFont bitmapFont= Load(fontFile);

        string exportPath = rootPath + "/" + Path.GetFileNameWithoutExtension(selected.name);

        BitmapFontToUnityFont.Work(bitmapFont, exportPath);
    }

    public static BitmapFont Load(string fontFile)
    {
        XmlDocument doc = new XmlDocument();
        if (File.Exists(fontFile))
        {
            doc.Load(fontFile);
        }
        else
        {
            TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>(fontFile);
            if (text)
            {
                doc.LoadXml(text.text);
            }
            else
            {
                Debug.LogError("No font file find. " + fontFile);
            }
        }

        return Load(doc, fontFile);
    }

    public static BitmapFont Load(XmlDocument doc, string fontFile)
    {
        BitmapXMLReader bitmapXmlReader = new BitmapXMLReader();

        BitmapFont fnt = bitmapXmlReader.Load(doc);

        BitmapParser bitmapParser = new BitmapParser();
        bitmapParser.Parse(fnt, fontFile);
        return fnt;
    }   
}