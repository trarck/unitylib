using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using System.Xml;

using YH.Font;

public static class BitmapFontImporter2 {
 
    [MenuItem("Assets/BitmapFont/Import Font")]
    public static void GenerateFont()
    {
        TextAsset selected = (TextAsset)Selection.activeObject;

        if (!selected) throw new UnityException(selected.name + "is not a valid font-xml file");
        string fontFile = AssetDatabase.GetAssetPath(selected);
        string rootPath = Path.GetDirectoryName(fontFile);

        BitmapParser bitmapParser = new BitmapParser();
        BitmapFont bitmapFont= bitmapParser.Parse(fontFile);

        string exportPath = rootPath + "/" + Path.GetFileNameWithoutExtension(selected.name);

        Work(bitmapFont, exportPath);
    } 
 
    private static void Work(BitmapFont bitmapFont, string exportPath)
    {    
 
        Font font = new Font();

        Texture2D texture = bitmapFont.pageAtlas;
        float texW = texture.width;
        float texH = texture.height;


        CharacterInfo[] charInfos = new CharacterInfo[bitmapFont.chars.Length];
        Rect r;

        for (int i = 0; i < bitmapFont.chars.Length; i++)
        {
            BitmapChar charNode = bitmapFont.chars[i];
            CharacterInfo charInfo = new CharacterInfo();

            charInfo.index = charNode.id;
            charInfo.advance = (int)charNode.xAdvance;

            Rect pageOffset = bitmapFont.pageOffsets[charNode.page];

            r = new Rect();
            r.x = charNode.position.x / texW;
            r.y = charNode.position.y / texH;
            r.width = charNode.size.x / texW;
            r.height = charNode.size.y / texH;
            r.y = 1f - r.y - r.height;

            r.position += pageOffset.position;

            charInfo.uvBottomLeft = new Vector2(r.xMin, r.yMax);
            charInfo.uvBottomRight = new Vector2(r.xMax, r.yMax);
            charInfo.uvTopLeft = new Vector2(r.xMin, r.yMin);
            charInfo.uvTopRight = new Vector2(r.xMax, r.yMin);



            r = new Rect();
            r.x = charNode.offset.x;
            r.y = charNode.offset.y;
            r.width = charNode.size.x;
            r.height = charNode.size.y;
            r.y = -r.y;
            r.height = -r.height;

            charInfo.minX = (int)r.xMin;
            charInfo.minY = (int)r.yMin;
            charInfo.maxX = (int)r.xMax;
            charInfo.maxY = (int)r.yMax;

            charInfos[i] = charInfo;
        }
 
        // Create material
        Shader shader = Shader.Find("UI/Default");
        Material material = new Material(shader);
        material.mainTexture = texture;
        AssetDatabase.CreateAsset(material, exportPath + ".mat");
 
        // Create font
        font.material = material;
        font.name = bitmapFont.face;
        font.characterInfo = charInfos;
        AssetDatabase.CreateAsset(font, exportPath + ".fontsettings");
    }
 
    private static int ToInt(XmlNode node, string name)
    {
        return Convert.ToInt32(node.Attributes.GetNamedItem(name).InnerText);
    }
}