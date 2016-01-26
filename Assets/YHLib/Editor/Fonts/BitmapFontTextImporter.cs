using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using System.Xml;

using YH.Fonts;

public static class BitmapFontTextImporter {
 
    [MenuItem("Assets/BitmapFont/Import Text Font")]
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
        BitmapTxtReader bitmapTxtReader = new BitmapTxtReader();

        BitmapFont fnt = bitmapTxtReader.Load(fontFile);

        BitmapParser bitmapParser = new BitmapParser();
        bitmapParser.Parse(fnt, fontFile);
        return fnt;
    }
}