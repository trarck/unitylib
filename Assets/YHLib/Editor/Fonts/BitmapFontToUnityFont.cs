using UnityEngine;
using System;
using UnityEditor;
using System.Xml;

using YH.Fonts;

public class BitmapFontToUnityFont
{
    public static void Work(BitmapFont bitmapFont, string exportPath)
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
                    pageOffset = bitmapFont.pageOffsets[charNode.page * 4 + charNode.chnl >> 1];
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

    static void SetFontPrivateProperty(Font font, BitmapFont bitmapFont)
    {
        Editor editor = Editor.CreateEditor(font);

        //lineSpacing
        SerializedProperty lineSpacing = editor.serializedObject.FindProperty("m_LineSpacing");
        lineSpacing.floatValue = bitmapFont.lineHeight;

        //fontSize
        SerializedProperty fontSize = editor.serializedObject.FindProperty("m_FontSize");
        fontSize.floatValue = bitmapFont.size;

        //font names
        SerializedProperty fontNamesValue = editor.serializedObject.FindProperty("m_FontNames");
        SetFontNames(fontNamesValue, bitmapFont);

        //KerningValues
        SerializedProperty kerningValues = editor.serializedObject.FindProperty("m_KerningValues");
        SetFontKernings(kerningValues, bitmapFont);

        editor.serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }

    static void SetFontNames(SerializedProperty fontNamesValue, BitmapFont bitmapFont)
    {
        fontNamesValue.InsertArrayElementAtIndex(0);
        //array
        fontNamesValue.Next(true);
        //Debug.Log(fontNamesValue.name + "[" + fontNamesValue.depth + "]" + "," + fontNamesValue.type + "," + fontNamesValue.hasChildren);
        //size
        fontNamesValue.Next(true);
        // Debug.Log(fontNamesValue.name + "[" + fontNamesValue.depth + "]" + "," + fontNamesValue.type + "," + fontNamesValue.hasChildren);

        //item
        fontNamesValue.Next(true);
        //Debug.Log(fontNamesValue.name + "[" + fontNamesValue.depth + "]" + "," + fontNamesValue.type + "," + fontNamesValue.hasChildren);
        fontNamesValue.stringValue = bitmapFont.face;
    }

    static void SetFontKernings(SerializedProperty kerningValues, BitmapFont bitmapFont)
    {
        if (kerningValues.arraySize > bitmapFont.kernings.Length)
        {
            //Debug.Log("remove:" + (kerningValues.arraySize - bitmapFont.kernings.Length));
            //移除多的key
            for (int i = kerningValues.arraySize - 1; i >= bitmapFont.kernings.Length - 1; --i)
            {
                kerningValues.DeleteArrayElementAtIndex(i);
            }
        }
        else if (kerningValues.arraySize < bitmapFont.kernings.Length)
        {
            //Debug.Log("add:" + (bitmapFont.kernings.Length-kerningValues.arraySize));
            //添加key
            for (int i = kerningValues.arraySize; i < bitmapFont.kernings.Length; ++i)
            {
                kerningValues.InsertArrayElementAtIndex(i);
            }
        }

        //设置每个元素值
        SerializedProperty prop = kerningValues;

        //array prop
        prop.Next(true);
        //size prop;
        prop.Next(true);

        for (int i = 0; i < bitmapFont.kernings.Length; ++i)
        {
            SetFontKerningItem(prop, bitmapFont.kernings[i]);
        }
    }

    static void SetFontKerningItem(SerializedProperty item, BitmapCharKerning kerning)
    {
        //data prop map< pair<ushort,ushort>,float >
        item.Next(true);
        //Debug.Log(item.name + "[" + item.depth + "]" + "," + item.type + "," + item.hasChildren);
        //key pair <ushort,ushort> first,second
        item.Next(true);
        //前面都是内部数据，跳过
        //Debug.Log(item.name + "[" + item.depth + "]" + "," + item.type + "," + item.hasChildren);
        //key first
        item.Next(true);
        //Debug.Log(item.name + "[" + item.depth + "]" + "," + item.type + "," + item.hasChildren);

        item.intValue = kerning.firstChar;
        //key second
        item.Next(true);
        //Debug.Log(item.name + "[" + item.depth + "]" + "," + item.type + "," + item.hasChildren);

        item.intValue = kerning.secondChar;
        //second
        item.Next(true);
        //Debug.Log(item.name + "[" + item.depth + "]" + "," + item.type + "," + item.hasChildren);

        item.floatValue = kerning.amount;
    }
}