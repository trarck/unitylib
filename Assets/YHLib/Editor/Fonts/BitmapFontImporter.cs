using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using System.Xml;

using YH.Font;

public static class BitmapFontImporter {
 
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

            

            r = new Rect();
            r.x = charNode.position.x / bitmapFont.scaleW;
            r.y = charNode.position.y / bitmapFont.scaleH;
            r.width = charNode.size.x / bitmapFont.scaleW;
            r.height = charNode.size.y / bitmapFont.scaleH;
            r.y = 1f - r.y - r.height;

            if (bitmapFont.pageOffsets != null)
            {
                Rect pageOffset;
                if (charNode.chnl == 15)
                {
                    pageOffset = bitmapFont.pageOffsets[charNode.page];      
                }
                else
                {
                    pageOffset = bitmapFont.pageOffsets[charNode.page*4+charNode.chnl>>1];
                    //r.x = r.x * pageOffset.width + pageOffset.xMin;
                    //r.y = r.y * pageOffset.height + pageOffset.yMin;
                    //r.width *= pageOffset.width;
                    //r.height *= pageOffset.height;
                }

                r.x = r.x * pageOffset.width + pageOffset.xMin;
                r.y = r.y * pageOffset.height + pageOffset.yMin;
                r.width *= pageOffset.width;
                r.height *= pageOffset.height;

            }

            charInfo.uvBottomLeft = new Vector2(r.xMin, r.yMin);
            charInfo.uvBottomRight = new Vector2(r.xMax, r.yMin);
            charInfo.uvTopLeft = new Vector2(r.xMin, r.yMax);
            charInfo.uvTopRight = new Vector2(r.xMax, r.yMax);



            r = new Rect();
            r.x = charNode.offset.x;
            r.y = charNode.offset.y;
            r.width = charNode.size.x;
            r.height = charNode.size.y;
            r.y = -r.y;
            r.height = -r.height;

            //charInfo.minX = (int)r.xMin;
            //charInfo.minY = -(int)r.yMin;
            //charInfo.maxX = (int)r.xMax;
            //charInfo.maxY = -(int)r.yMax;
            charInfo.vert = r;
            //Rect t = new Rect();
            //t.xMin = r.xMin;
            //t.xMax = r.xMax;
            //t.yMin = r.yMin;
            //t.yMax = r.yMax;

            //Debug.Log(charNode.id+","+ t.x + "," + t.y + "," + t.width + "," + t.height);

            charInfos[i] = charInfo;
        }
 
        // Create material
        Shader shader = Shader.Find("GUI/Text Shader");
        Material material = new Material(shader);
        material.mainTexture = texture;
        AssetDatabase.CreateAsset(material, exportPath + ".mat");
 
        // Create font
        font.material = material;
        font.name = bitmapFont.face;
        font.characterInfo = charInfos;
        AssetDatabase.CreateAsset(font, exportPath + ".fontsettings");

        SetFontPrivateProperty(font, bitmapFont);
    }

    static void SetFontPrivateProperty(Font font,BitmapFont bitmapFont)
    {
        Editor editor = Editor.CreateEditor(font);

        //lineSpacing
        SerializedProperty lineSpacing = editor.serializedObject.FindProperty("m_LineSpacing");
        lineSpacing.floatValue = bitmapFont.lineHeight;

        //fontSize
        SerializedProperty fontSize = editor.serializedObject.FindProperty("m_FontSize");
        fontSize.floatValue = bitmapFont.size;
        editor.serializedObject.ApplyModifiedProperties();

        AssetDatabase.SaveAssets();
    }

    static void SetFontKerning(Font font,BitmapFont bitmapFont)
    {
        Editor editor = Editor.CreateEditor(font);

        //lineSpacing
        SerializedProperty kerningValues = editor.serializedObject.FindProperty("m_KerningValues");
        kerningValues.floatValue = bitmapFont.lineHeight;
    }

    private static int ToInt(XmlNode node, string name)
    {
        return Convert.ToInt32(node.Attributes.GetNamedItem(name).InnerText);
    }
}