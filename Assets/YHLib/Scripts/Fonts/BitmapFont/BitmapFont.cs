using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

namespace YH.Fonts
{

    [System.Serializable]
    public class BitmapChar
    {
        public int id;
        public Vector2 position;
        public Vector2 size;
        public Vector2 offset;
        public int page;
        public int xAdvance;
        public int chnl;
    }

    [System.Serializable]
    public class BitmapInfo
    {

    }

    [System.Serializable]
    public class BitmapCharKerning
    {
        public int firstChar;
        public int secondChar;
        public float amount;
    }

    [System.Serializable]
    public class BitmapRange
    {
        public BitmapRange(int l,int t,int r,int b)
        {
            left = l;
            top = t;
            right = r;
            bottom = b;
        }

        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public enum ChnlValues
    {
        Glyph,
        Outline,
        GlyphOutline,
        Zero,
        One,
    }

    public class BitmapFont
    {
        string m_Face;

        float m_Size;

        bool m_Bold;

        bool m_Italic;

        int m_Charset;

        bool m_Unicode;

        float m_StretchH;

        bool m_Smooth;

        int m_Aa;

        BitmapRange m_Padding;

        Vector2 m_Spacing;

        bool m_Outline;


        float m_LineHeight;

        float m_Base;

        float m_ScaleW;

        float m_ScaleH;

        bool m_Packed;

        ChnlValues m_AlphaChnl;

        ChnlValues m_RedChnl;

        ChnlValues m_GreenChnl;

        ChnlValues m_BlueChnl;

        BitmapChar[] m_Chars;

        string[] m_Pages;

        Rect[] m_PageOffsets;

        Texture2D m_PageAtlas;

        BitmapCharKerning[] m_Kernings;

        public BitmapChar GetBitmapChar(int c)
        {
            foreach (BitmapChar bitmapChar in m_Chars)
            {
                if (c == bitmapChar.id)
                {
                    return bitmapChar;
                }
            }
            Debug.LogWarning("Could not find bitmap character for unicode char " + c);
            return m_Chars[0];
        }

        public Rect GetUVRect(int c)
        {
            BitmapChar bitmapChar = GetBitmapChar(c);
            return GetUVRect(bitmapChar);
        }

        public Rect GetUVRect(BitmapChar bitmapChar)
        {
            //Convert positions/scale from AngleCode-format (pixels, top left origin) to uv format (0-1, bottom left origin)
            Vector2 scaledSize = new Vector2(bitmapChar.size.x / m_ScaleW, bitmapChar.size.y / m_ScaleH);
            Vector2 scaledPos = new Vector2(bitmapChar.position.x / m_ScaleW, bitmapChar.position.y / m_ScaleH);
            Vector2 uvCharPos = new Vector2(scaledPos.x, 1 - (scaledPos.y + scaledSize.y));

            //Scale and translate according to page atlas
            Rect offset = m_PageOffsets[bitmapChar.page];
            uvCharPos = new Vector2(uvCharPos.x * offset.width + offset.xMin, uvCharPos.y * offset.height + offset.yMin);
            scaledSize = new Vector2(scaledSize.x * offset.width, scaledSize.y * offset.height);

            return new Rect(uvCharPos.x, uvCharPos.y, scaledSize.x, scaledSize.y);
        }

        public float GetKerning(char first, char second)
        {
            if (m_Kernings != null)
            {
                foreach (BitmapCharKerning krn in m_Kernings)
                {
                    if (krn.firstChar == (int)first && krn.secondChar == (int)second)
                    {
                        return krn.amount;
                    }
                }
            }
            return 0;
        }

        public Vector2 CalculateSize(string str, Vector2 renderSize)
        {
            Vector2 curPos = new Vector2(0, renderSize.y);
            Vector2 scale = renderSize / m_Size;

            for (int idx = 0; idx < str.Length; idx++)
            {
                char c = str[idx];
                BitmapChar charInfo = GetBitmapChar((int)c);

                float krn = 0;
                if (idx < str.Length - 1)
                {
                    krn = GetKerning(c, str[idx + 1]);
                }
                curPos.x += (charInfo.xAdvance + krn) * scale.x;
            }

            return curPos;
        }

        public Vector2 CalculateSize(string str, float renderSize)
        {
            return CalculateSize(str, new Vector2(renderSize, renderSize));
        }
        
        //==========================generate auto==============================//
        public float size
        {
            set
            {
                m_Size = value;
            }

            get
            {
                return m_Size;
            }
        }

        public float lineHeight
        {
            set
            {
                m_LineHeight = value;
            }

            get
            {
                return m_LineHeight;
            }
        }

        public float baseSize
        {
            set
            {
                m_Base = value;
            }

            get
            {
                return m_Base;
            }
        }

        public float scaleW
        {
            set
            {
                m_ScaleW = value;
            }

            get
            {
                return m_ScaleW;
            }
        }

        public float scaleH
        {
            set
            {
                m_ScaleH = value;
            }

            get
            {
                return m_ScaleH;
            }
        }

        public BitmapChar[] chars
        {
            set
            {
                m_Chars = value;
            }

            get
            {
                return m_Chars;
            }
        }

        public Rect[] pageOffsets
        {
            set
            {
                m_PageOffsets = value;
            }

            get
            {
                return m_PageOffsets;
            }
        }

        public Texture2D pageAtlas
        {
            set
            {
                m_PageAtlas = value;
            }

            get
            {
                return m_PageAtlas;
            }
        }

        public BitmapCharKerning[] kernings
        {
            set
            {
                m_Kernings = value;
            }

            get
            {
                return m_Kernings;
            }
        }

        public string face
        {
            set
            {
                m_Face = value;
            }

            get
            {
                return m_Face;
            }
        }

        public bool bold
        {
            set
            {
                m_Bold = value;
            }

            get
            {
                return m_Bold;
            }
        }

        public bool italic
        {
            set
            {
                m_Italic = value;
            }

            get
            {
                return m_Italic;
            }
        }

        public int charset
        {
            set
            {
                m_Charset = value;
            }

            get
            {
                return m_Charset;
            }
        }

        public bool unicode
        {
            set
            {
                m_Unicode = value;
            }

            get
            {
                return m_Unicode;
            }
        }

        public float stretchH
        {
            set
            {
                m_StretchH = value;
            }

            get
            {
                return m_StretchH;
            }
        }

        public bool smooth
        {
            set
            {
                m_Smooth = value;
            }

            get
            {
                return m_Smooth;
            }
        }

        public int aa
        {
            set
            {
                m_Aa = value;
            }

            get
            {
                return m_Aa;
            }
        }

        public BitmapRange padding
        {
            set
            {
                m_Padding = value;
            }

            get
            {
                return m_Padding;
            }
        }

        public Vector2 spacing
        {
            set
            {
                m_Spacing = value;
            }

            get
            {
                return m_Spacing;
            }
        }

        public bool outline
        {
            set
            {
                m_Outline = value;
            }

            get
            {
                return m_Outline;
            }
        }

        public bool packed
        {
            set
            {
                m_Packed = value;
            }

            get
            {
                return m_Packed;
            }
        }

        public ChnlValues alphaChnl
        {
            set
            {
                m_AlphaChnl = value;
            }

            get
            {
                return m_AlphaChnl;
            }
        }

        public ChnlValues redChnl
        {
            set
            {
                m_RedChnl = value;
            }

            get
            {
                return m_RedChnl;
            }
        }

        public ChnlValues greenChnl
        {
            set
            {
                m_GreenChnl = value;
            }

            get
            {
                return m_GreenChnl;
            }
        }

        public ChnlValues blueChnl
        {
            set
            {
                m_BlueChnl = value;
            }

            get
            {
                return m_BlueChnl;
            }
        }

        public string[] pages
        {
            set
            {
                m_Pages = value;
            }

            get
            {
                return m_Pages;
            }
        }
    }
}