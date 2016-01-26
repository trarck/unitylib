using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

namespace YH.Fonts
{
    public class BitmapTxtReader
    {
        public BitmapFont Load(string fontFile)
        {
            string content = "";
            if (File.Exists(fontFile))
            {
                content = File.ReadAllText(fontFile);
            }

            return Pause(content);
        }

        public BitmapFont Pause(string content)
        {
            char[] lineSeparator = new char[] {'\r','\n' };
            char[] separator = new char[] { ' ' };

            BitmapFont fnt = new BitmapFont();

            List<string> pages = new List<string>();
            List<BitmapChar> chars = new List<BitmapChar>();
            List<BitmapCharKerning> kernings = new List<BitmapCharKerning>();

            string[] lines = content.Split(lineSeparator, System.StringSplitOptions.RemoveEmptyEntries);
            Debug.Log(lines.Length);
            foreach (string line in lines)
            {
                string[] items = line.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                switch (items[0])
                {
                    case "info":
                        ReadInfo(fnt, items);
                        break;
                    case "common":
                        ReadCommon(fnt, items);
                        break;
                    case "page":
                        ReadPage(pages, items);
                        break;
                    case "chars":
                        break;
                    case "char":
                        ReadCharacter(chars, items);
                        break;
                    case "kernings":
                        break;
                    case "kerning":
                        break;
                }
            }

            fnt.pages = pages.ToArray();
            fnt.chars = chars.ToArray();
            fnt.kernings = kernings.ToArray();

            return fnt;
        }

        void ReadInfo(BitmapFont fnt, string[] infos)
        {
            foreach (string item in infos)
            {
                object value = "";
                string key = GetKeyValue(item, out value);

                switch (key)
                {
                    case "face":
                        fnt.face = (value as string).Replace("\"","");
                        break;
                    case "size":
                        fnt.size = Mathf.Abs(Convert.ToInt32(value));
                        break;
                    case "bold":
                        fnt.bold = ReadBool(value as string);
                        break;
                    case "italic":
                        fnt.italic = ReadBool(value as string);
                        break;
                    case "charset":
                        fnt.charset = (value as string).Replace("\"", "");
                        break;
                    case "unicode":
                        fnt.unicode = ReadBool(value as string);
                        break;
                    case "stretchH":
                        fnt.stretchH = Convert.ToInt32(value);
                        break;
                    case "smooth":
                        fnt.smooth = ReadBool(value as string);
                        break;
                    case "aa":
                        fnt.aa = Convert.ToInt32(value);
                        break;
                    case "padding":
                        fnt.padding = ReadBitmapRange(value as string);
                        break;
                    case "spacing":
                        fnt.spacing = ReadVector2(value as string);
                        break;
                    case "outline":
                        fnt.outline = ReadBool(value as string);
                        break;
                }
            }
        }

        void ReadCommon(BitmapFont fnt, string[] commons)
        {
            foreach (string item in commons)
            {
                object value = "";
                string key = GetKeyValue(item, out value);
                switch (key)
                {
                    case "lineHeight":
                        fnt.lineHeight = Convert.ToInt32(value);
                        break;
                    case "base":
                        fnt.baseSize = Convert.ToSingle(value);
                        break;
                    case "scaleW":
                        fnt.scaleW = Convert.ToSingle(value);
                        break;
                    case "scaleH":
                        fnt.scaleH = Convert.ToSingle(value);
                        break;
                    case "packed":
                        fnt.packed = ReadBool(value as string);
                        break;
                    case "alphaChnl":
                        fnt.alphaChnl = (ChnlValues)Convert.ToInt32(value);
                        break;
                    case "redChnl":
                        fnt.redChnl = (ChnlValues)Convert.ToInt32(value);
                        break;
                    case "greenChnl":
                        fnt.greenChnl = (ChnlValues)Convert.ToInt32(value);
                        break;
                    case "blueChnl":
                        fnt.blueChnl = (ChnlValues)Convert.ToInt32(value);
                        break;
                }
            }
        }

        void ReadCharacter(List<BitmapChar> chars, string[] charData)
        {
            BitmapChar bitmapChar = new BitmapChar();

            foreach (string item in charData)
            {
                object value = "";
                string key = GetKeyValue(item, out value);

                switch (key)
                {
                    case "id":
                        bitmapChar.id = Convert.ToInt32(value);
                        break;
                    case "page":
                        bitmapChar.page = Convert.ToInt32(value);
                        break;
                    case "chnl":
                        bitmapChar.chnl = Convert.ToInt32(value);
                        break;
                    case "xadvance":
                        bitmapChar.xAdvance = Convert.ToInt32(value);
                        break;
                    case "x":
                        bitmapChar.position.x = float.Parse(value as string, System.Globalization.NumberFormatInfo.InvariantInfo);
                        break;
                    case "y":
                        bitmapChar.position.y = float.Parse(value as string, System.Globalization.NumberFormatInfo.InvariantInfo);
                        break;
                    case "width":
                        bitmapChar.size.x = float.Parse(value as string, System.Globalization.NumberFormatInfo.InvariantInfo);
                        break;
                    case "height":
                        bitmapChar.size.y = float.Parse(value as string, System.Globalization.NumberFormatInfo.InvariantInfo);
                        break;
                    case "xoffset":
                        bitmapChar.offset.x = float.Parse(value as string, System.Globalization.NumberFormatInfo.InvariantInfo);
                        break;
                    case "yoffset":
                        bitmapChar.offset.y = float.Parse(value as string, System.Globalization.NumberFormatInfo.InvariantInfo);
                        break;
                }
            }
            chars.Add(bitmapChar);
        }

        void ReadPage(List<string> pages, string[] page)
        {
            foreach (string item in page)
            {
                object value = "";
                string key = GetKeyValue(item, out value);
                if (key == "file")
                {
                    pages.Add((value as string).Replace("\"", ""));
                    break;
                }
            }
        }

        void ReadKerning(List<BitmapCharKerning> kernings, string[] kerningData)
        {
            BitmapCharKerning kerning = new BitmapCharKerning();

            foreach (string item in kerningData)
            {
                object value = "";
                string key = GetKeyValue(item, out value);

                switch (key)
                {
                    case "first":
                        kerning.firstChar = Convert.ToInt32(value);
                        break;
                    case "second":
                        kerning.secondChar = Convert.ToInt32(value);
                        break;
                    case "amount":
                        kerning.amount = Convert.ToInt32(value);
                        break;
                }
            }
        }

        static string GetKeyValue(string s, out object value)
        {
            int idx = s.IndexOf('=');
            if (idx == -1)
            {
                value = "";
                return "";
            }
            else
            {
                value = s.Substring(idx + 1);
                return s.Substring(0, idx);
            }
        }

        static string GetString(string s)
        {
            int idx = s.IndexOf('=');
            return (idx == -1) ? "" : s.Substring(idx + 1);
        }

        static int GetInt(string s)
        {
            int val = 0;
            string text = GetString(s);
#if UNITY_FLASH
		try { val = int.Parse(text); } catch (System.Exception) { }
#else
            int.TryParse(text, out val);
#endif
            return val;
        }

        static bool ReadBool(string s)
        {
            return s == "1";
        }

        static Vector2 ReadVector2(string s)
        {
            string[] arr = s.Split(',');
            return new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
        }

        static BitmapRange ReadBitmapRange(string s)
        {
            string[] arr = s.Split(',');
            return new BitmapRange(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), int.Parse(arr[3]));
        }
    }
}