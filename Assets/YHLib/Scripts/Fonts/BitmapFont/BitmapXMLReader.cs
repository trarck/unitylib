using UnityEngine;
using System.Xml;
using System.IO;

namespace YH.Font
{
    public class BitmapXMLReader
    {
        public BitmapFont Load(string fontFile)
        {
            XmlDocument doc = new XmlDocument();

            if (File.Exists(fontFile))
            {
                doc.Load(fontFile);
            }

            return Load(doc);
        }

        public BitmapFont Load(XmlDocument doc)
        {
            BitmapFont fnt = new BitmapFont();

            //Read font info          
            XmlNode info = doc.SelectSingleNode("/font/info");
            ReadInfo(fnt, info);

            //Read commmon
            XmlNode common = doc.SelectSingleNode("/font/common");
            ReadCommon(fnt, common);

            //Read character info
            XmlNodeList chars = doc.SelectNodes("/font/chars/char");
            ReadCharacters(fnt, chars);

            //Load texture pages and convert to distance fields
            XmlNodeList pages = doc.SelectNodes("/font/pages/page");
            ReadPages(fnt, pages);

            //Load kerning info
            XmlNodeList kernings = doc.SelectNodes("/font/kernings/kerning");
            ReadKernings(fnt, kernings);

            return fnt;
        }

        void ReadInfo(BitmapFont fnt, XmlNode info)
        {
            fnt.face = info.Attributes["face"].Value;
            fnt.size = Mathf.Abs(ReadFloatAttribute(info, "size"));
            fnt.bold = ReadBoolAttribute(info, "bold");
            fnt.italic = ReadBoolAttribute(info, "italic");
            fnt.charset = ReadIntAttribute(info, "charset");
            fnt.unicode = ReadBoolAttribute(info, "unicode");
            fnt.stretchH = ReadFloatAttribute(info, "stretchH");
            fnt.smooth = ReadBoolAttribute(info, "smooth");
            fnt.aa = ReadIntAttribute(info, "aa");
            fnt.padding = ReadBitmapRangeAttributes(info, "padding");
            fnt.spacing = ReadVector2Attributes(info, "spacing");
            fnt.outline = ReadBoolAttribute(info, "outline");
        }

        void ReadCommon(BitmapFont fnt, XmlNode common)
        {
            fnt.lineHeight = ReadFloatAttribute(common, "lineHeight");
            fnt.baseSize = ReadFloatAttribute(common, "base");
            fnt.scaleH = ReadFloatAttribute(common, "scaleW");
            fnt.scaleW = ReadFloatAttribute(common, "scaleH");
            fnt.packed = ReadBoolAttribute(common, "packed");
            fnt.alphaChnl = (ChnlValues)ReadIntAttribute(common, "alphaChnl");
            fnt.redChnl = (ChnlValues)ReadIntAttribute(common, "redChnl");
            fnt.greenChnl = (ChnlValues)ReadIntAttribute(common, "greenChnl");
            fnt.blueChnl = (ChnlValues)ReadIntAttribute(common, "blueChnl");
        }

        void ReadCharacters(BitmapFont fnt, XmlNodeList chars)
        {
            fnt.chars = new BitmapChar[chars.Count];
            int index = 0;
            foreach (XmlNode char_node in chars)
            {
                fnt.chars[index] = new BitmapChar();
                fnt.chars[index].id = ReadIntAttribute(char_node, "id");
                fnt.chars[index].position = ReadVector2Attributes(char_node, "x", "y");
                fnt.chars[index].size = ReadVector2Attributes(char_node, "width", "height");
                fnt.chars[index].offset = ReadVector2Attributes(char_node, "xoffset", "yoffset");
                fnt.chars[index].xAdvance = ReadIntAttribute(char_node, "xadvance");
                fnt.chars[index].page = ReadIntAttribute(char_node, "page");
                fnt.chars[index].chnl = ReadIntAttribute(char_node, "chnl");
                index++;
            }
        }

        void ReadPages(BitmapFont fnt, XmlNodeList pages)
        {
            //load texture
            fnt.pages = new string[pages.Count];
            int index = 0;
            foreach (XmlNode page in pages)
            {
                //Find original font texture
                string imagePath = page.Attributes["file"].Value;
                fnt.pages[index] = imagePath;
                index++;
            }
        }

        void ReadKernings(BitmapFont fnt, XmlNodeList kernings)
        {
            fnt.kernings = new BitmapCharKerning[kernings.Count];
            int index = 0;
            foreach (XmlNode kerning in kernings)
            {
                BitmapCharKerning krn = new BitmapCharKerning();
                krn.firstChar = ReadIntAttribute(kerning, "first");
                krn.secondChar = ReadIntAttribute(kerning, "second");
                krn.amount = ReadFloatAttribute(kerning, "amount");
                fnt.kernings[index] = krn;
                index++;
            }            
        }

        static int ReadIntAttribute(XmlNode node, string attribute)
        {
            return node.Attributes[attribute].Value!=""?int.Parse(node.Attributes[attribute].Value, System.Globalization.NumberFormatInfo.InvariantInfo):0;
        }
        static float ReadFloatAttribute(XmlNode node, string attribute)
        {
            return float.Parse(node.Attributes[attribute].Value, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        static bool ReadBoolAttribute(XmlNode node, string attribute)
        {
            return node.Attributes[attribute].Value == "1";
        }

        static Vector2 ReadVector2Attributes(XmlNode node, string attributeX, string attributeY)
        {
            return new Vector2(ReadFloatAttribute(node, attributeX), ReadFloatAttribute(node, attributeY));
        }

        static Vector2 ReadVector2Attributes(XmlNode node, string attribute)
        {
            string value = node.Attributes[attribute].Value;
            string[] arr = value.Split(',');
            return new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
        }

        static BitmapRange ReadBitmapRangeAttributes(XmlNode node, string attribute)
        {
            string value = node.Attributes[attribute].Value;
            string[] arr = value.Split(',');
            return new BitmapRange(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), int.Parse(arr[3]));
        }
    }
}