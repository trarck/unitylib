using System.Collections;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace TK.Excel
{
    public class ReadLinkHelper
    {
        /// <summary>
        /// 解析出目标位置
        /// A1B2;A1,B2;A1,2;1,B2;1,2
        /// </summary>
        /// <param name="posString"></param>
        /// <returns></returns>
        public static CellPosition GetCellPosition(string posString)
        {
            CellPosition cp = new CellPosition();

            int colStart = 0;
            int colEnd = 0;
            int rowStart = 0;
            int rowEnd = 0;

            int i = 0;

            //col start
            for (; i < posString.Length; ++i)
            {
                char c = posString[i];
                if ('a' <= c && c <= 'z')
                {
                    colStart = colStart * 26 + c - 'a'+1;
                }
                else if ('A' <= c && c <= 'Z')
                {
                    colStart = colStart * 26 + c - 'A'+1;
                }
                else
                {
                    break;
                }
            }
            //从0开始
            if (colStart == 0)
            {
                cp.colStart = 0;
            }
            else
            {
                cp.colStart = colStart - 1;
            }

            //row start
            for (; i < posString.Length; ++i)
            {
                char c = posString[i];
                if ('0' <= c && c <= '9')
                {
                    rowStart = rowStart * 10 + c - '0';
                }
                else
                {
                    if (c == ',')
                    {
                        ++i;
                    }
                    break;
                }
            }
            //从0开始
            if (rowStart == 0)
            {
                cp.rowStart = 0;
            }
            else
            {
                cp.rowStart = rowStart - 1;
            }

            //col end
            for (; i < posString.Length; ++i)
            {
                char c = posString[i];
                if ('a' <= c && c <= 'z')
                {
                    colEnd = colEnd * 26 + c - 'a' + 1;
                }
                else if ('A' <= c && c <= 'Z')
                {
                    colEnd = colEnd * 26 + c - 'A' + 1;
                }
                else
                {
                    break;
                }
            }
            //从0开始。
            cp.colEnd = colEnd -1;

            //end row
            for (; i < posString.Length; ++i)
            {
                char c = posString[i];
                if ('0' <= c && c <= '9')
                {
                    rowEnd = rowEnd * 10 + c - '0';
                }
                else
                {
                    break;
                }
            }
            //索引号从0开始
            cp.rowEnd = rowEnd - 1;
            return cp;
        }

        public static string ParseLinkCell(ICell cell, out CellPosition cp)
        {
            string key;
            return ParseLinkCell(cell, out cp, out key);
        }

        public static string ParseLinkCell(ICell cell, out CellPosition start,out string key)
        {
            string linkWhere = cell.StringCellValue;

            string linkSheetName = "";

            int pos = linkWhere.IndexOf("!");
            if (pos > -1)
            {
                //表的开始位置
                int endPos = linkWhere.IndexOf(":");
                string linkCellPositionStr = null;
                if (endPos > -1) {
                    linkCellPositionStr = linkWhere.Substring(pos + 1,endPos-pos-1);
                }
                else
                {
                    linkCellPositionStr = linkWhere.Substring(pos + 1);
                }
                start = GetCellPosition(linkCellPositionStr);

                linkSheetName = linkWhere.Substring(0, pos);
            }
            else
            {
                //第一列，第一行
                start = new CellPosition();
                start.rowStart = 0;
                start.colStart = 0;
                start.rowEnd = -1;
                linkSheetName = linkWhere;
            }

            pos = linkWhere.IndexOf(":");
            if (pos > -1)
            {
                key = linkWhere.Substring(pos+1);
            }
            else
            {
                key = null;
            }

            return linkSheetName;
        }
    }
}
