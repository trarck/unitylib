using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YH.UI
{
    public class BigTable : ScrollRect
    {
        public class Cell
        {
            public int row;
            public int col;
            public int index;
            public int tag;
            public RectTransform content;
            public int lastIndex = -1;
        }

        public interface IDataProvider
        {
            //总数量
            int count { get; }
            //开始元素号
            int startIndex { get; }
            //创建元素
            Cell CreateCell(int tag, BigTable list);
            //更新元素
            void UpdateCell(Cell cell);
        }

        protected IDataProvider m_DataProvider = null;
        public IDataProvider dataProvider
        {
            get
            {
                return m_DataProvider;
            }
            set
            {
                m_DataProvider = value;
                if (m_DataProvider != null)
                {
                    Init();
                }
            }
        }

        public RectOffset padding= null;
        public Vector2 spacing=Vector2.zero;
        //这个是显示数据，不应该放在dataprovider里。
        public Vector2 cellSize = Vector2.zero;
        //安全列或排
        public int safeRow = 2;
        protected int m_VisibleRow = 0;
        protected int m_VisibleColumn = 0;
        protected int m_RowAxis = 1;//y default is vertical
        protected int m_ColAxis = 0;//x default is vertical
        //当前显示的个数。
        protected int m_ShowCount = 0;

        //移动方向。1:正向，-1:负向,0:方向
        protected int m_MoveDirection = 0;
        //改变阀值。不能设置为0.5，否则可能会发生抖动。
        protected float m_Threshold = 0.6f;

        LinkedList<Cell> m_Cells;

        protected void InitCells()
        {
            if (m_Cells == null)
            {
                m_Cells = new LinkedList<Cell>();
            }
            else
            {
                m_Cells.Clear();
            }
            CalculateShowCount();
            CreateCells();
        }

        public virtual void Init()
        {
            InitCells();
            Refresh();
        }

        public virtual void Refresh()
        {
            AssignCells(m_DataProvider.startIndex);
            UpdateCells();
            //set content size
            ResizeContent();
        }

        protected void CalculateShowCount()
        {
            //计算要显示的元素个数
            if (vertical)
            {
                m_RowAxis = 1;
                m_ColAxis = 0;
                //m_VisibleRow = Mathf.CeilToInt(viewRect.rect.height / m_DataProvider.cellSize.y);
                //m_VisibleColumn = Mathf.CeilToInt(viewRect.rect.width / m_DataProvider.cellSize.x);
            }
            else if (horizontal)
            {
                m_RowAxis = 0;
                m_ColAxis = 1;
                //m_VisibleRow = Mathf.CeilToInt(viewRect.rect.width / m_DataProvider.cellSize.x);
                //m_VisibleColumn = Mathf.CeilToInt(viewRect.rect.height / m_DataProvider.cellSize.y);
            }
            m_VisibleRow = Mathf.CeilToInt(viewRect.rect.size[m_RowAxis] / cellSize[m_RowAxis]);
            m_VisibleColumn = Mathf.FloorToInt(viewRect.rect.size[m_ColAxis] / cellSize[m_ColAxis]);

            m_ShowCount = m_VisibleRow * m_VisibleColumn;
        }

        protected void ResizeContent()
        {
            int totalRow = Mathf.CeilToInt((float)m_DataProvider.count / m_VisibleColumn);
            
            if (vertical)
            {
                //set offset
                Vector3 pos = content.localPosition;
                int startRow = Mathf.FloorToInt(m_DataProvider.startIndex / m_VisibleColumn);
                pos.y = startRow>0?((cellSize.y + spacing.y) * startRow - spacing.y + padding.top):0;
                content.localPosition = pos;
                //set height
                float height = (cellSize.y + spacing.y) * totalRow - spacing.y;
                height += padding.top + padding.bottom;

                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
            else
            {
                //set offset
                Vector3 pos = content.localPosition;
                int startRow = Mathf.FloorToInt(m_DataProvider.startIndex / m_VisibleColumn);
                pos.x =startRow>0?( (cellSize.x + spacing.x) * startRow - spacing.x + padding.left):0;
                content.localPosition = pos;
                //set width
                float width = (cellSize.x + spacing.x) * totalRow - spacing.x;
                width += padding.left + padding.right;
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }
        }

        public override void OnScroll(PointerEventData data)
        {
            Vector2 oldPosition = content.anchoredPosition;
            base.OnScroll(data);
            Vector2 delta = content.anchoredPosition - oldPosition;
            LayoutCells(delta);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            Vector2 oldPosition = content.anchoredPosition;
            base.OnDrag(eventData);
            Vector2 delta = content.anchoredPosition - oldPosition;
            LayoutCells(delta);
        }

        protected override void LateUpdate()
        { 
 #if UNITY_EDITOR
            if (m_DataProvider == null)
            {
                return;
            }
#endif
            Vector2 oldPosition = content.anchoredPosition;
            base.LateUpdate();
            Vector2 delta = content.anchoredPosition - oldPosition;

            LayoutCells(delta);
        }

        #region Item

        private readonly Vector3[] m_ItemCorners = new Vector3[4];

        private Bounds GetItemBounds(RectTransform rectTransform)
        {
            if (rectTransform == null)
                return new Bounds();
            rectTransform.GetWorldCorners(m_ItemCorners);
            var viewWorldToLocalMatrix = viewRect.worldToLocalMatrix;
            return TransformBounds(m_ItemCorners, ref viewWorldToLocalMatrix);
        }

        internal static Bounds TransformBounds(Vector3[] corners, ref Matrix4x4 viewWorldToLocalMatrix)
        {
            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int j = 0; j < 4; j++)
            {
                Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4(corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        //protected Cell CreateItem(int row,int col,int index, int tag)
        //{
        //    Cell item = null;
        //    if (m_DataProvider != null)
        //    {
        //        item = m_DataProvider.CreateCell(index,tag, this);
        //    }
        //    else
        //    {
        //        item = new Cell();
        //    }

        //    if (item != null)
        //    {
        //        item.index = index;
        //        item.row = row;
        //        item.col = col;
        //        item.tag = tag;
        //        if (item.content != null && item.content.parent != content)
        //        {
        //            item.content.SetParent(content);
        //        }
        //    }

        //    return item;
        //}

        protected Cell CreateCell(int tag)
        {
            Cell item = null;
            if (m_DataProvider != null)
            {
                item = m_DataProvider.CreateCell(tag, this);
            }
            else
            {
                item = new Cell();
            }

            if (item != null)
            {
                item.tag = tag;
                if (item.content != null && item.content.parent != content)
                {
                    item.content.SetParent(content);
                }
            }
            return item;
        }

        protected void CreateCells()
        {
            int total = (safeRow * 2 + m_VisibleRow) * m_VisibleColumn;
            for(int i = 0; i < total; ++i)
            {
                Cell item = CreateCell(i);
                m_Cells.AddLast(item);
            }
        }

        //protected void CreateItems(int startIndex)
        //{
        //    if (startIndex < 0)
        //    {
        //        startIndex = 0;
        //    }

        //    int tagIndex = 0;
        //    //保证startIndex显示在第一排。
        //    int startRow = Mathf.FloorToInt(startIndex / m_VisibleColumn);

        //    int beforeRow = Mathf.Min(safeRow, startRow);
        //    int afterRow = safeRow - beforeRow + safeRow;
        //    //前面的每一排都要显示满，startIndex不一定出现在第一排的第一列，但要保证在第一排。
        //    //这里的startIndex>=0
        //    startIndex = startIndex - beforeRow * m_VisibleColumn;
        //    //before
        //    for (int j = 0; j < beforeRow; ++j)
        //    {
        //        for (int i = 0; i < m_VisibleColumn; ++i)
        //        {
        //            Cell item = CreateItem(startRow - beforeRow + j, i, startIndex++, tagIndex++);
        //            m_Cells.AddLast(item);
        //        }
        //    }
        //    //display
        //    for (int j = 0; j < m_VisibleRow; ++j)
        //    {
        //        for (int i = 0; i < m_VisibleColumn; ++i)
        //        {
        //            Cell item = CreateItem(startRow + j, i, startIndex++, tagIndex++);
        //            m_Cells.AddLast(item);
        //        }
        //    }
        //    //after
        //    for (int j = 0; j < afterRow; ++j)
        //    {
        //        for (int i = 0; i < m_VisibleColumn; ++i)
        //        {
        //            Cell item = CreateItem(startRow + m_VisibleRow + j, i, startIndex++, tagIndex++);
        //            m_Cells.AddLast(item);
        //        }
        //    }
        //}

        protected void AssignCells(int startIndex)
        {
            if (startIndex < 0)
            {
                startIndex = 0;
            }
                        
            var iter = m_Cells.First;
            //保证startIndex显示在第一排。
            int startRow = Mathf.FloorToInt(startIndex / m_VisibleColumn);

            int beforeRow = Mathf.Min(safeRow, startRow);
            int afterRow = safeRow - beforeRow + safeRow;
            //前面的每一排都要显示满，startIndex不一定出现在第一排的第一列，但要保证在第一排。
            //这里的startIndex>=0
            startIndex = startIndex - beforeRow * m_VisibleColumn;
            //before
            for (int j = 0; j < beforeRow; ++j)
            {
                for (int i = 0; i < m_VisibleColumn; ++i)
                {
                    Cell cell = iter.Value;
                    cell.row = startRow - beforeRow + j;
                    cell.col = i;
                    cell.index = startIndex++;
                    cell.lastIndex = -1;
                    iter = iter.Next;
                }
            }

            //display
            for (int j = 0; j < m_VisibleRow; ++j)
            {
                for (int i = 0; i < m_VisibleColumn; ++i)
                {
                    Cell cell = iter.Value;
                    cell.row = startRow + j;
                    cell.col = i;
                    cell.index = startIndex++;
                    cell.lastIndex = -1;
                    iter = iter.Next;
                }
            }
            //after
            for (int j = 0; j < afterRow; ++j)
            {
                for (int i = 0; i < m_VisibleColumn; ++i)
                {
                    Cell cell = iter.Value;
                    cell.row = startRow + m_VisibleRow + j;
                    cell.col = i;
                    cell.index = startIndex++;
                    cell.lastIndex = -1;
                    iter = iter.Next;
                }
            }
        }

        protected void UpdateCells()
        {
            foreach (Cell cell in m_Cells)
            {
                if (cell.index>= 0 && cell.index < m_DataProvider.count)
                {
                    //Debug.LogFormat("Update:{0},{1},{2},{3}", cell.index, cell.lastIndex, cell.row, cell.col);
                    if (cell.index != cell.lastIndex)
                    {
                        if (m_DataProvider != null)
                        {
                            m_DataProvider.UpdateCell(cell);
                            cell.lastIndex = cell.index;
                        }

                        //update position
                        if (vertical)
                        {
                            Vector2 pos = (cellSize + spacing) * new Vector2(cell.col, cell.row) - spacing + new Vector2(padding.left, padding.top);
                            //Vector2 pos = new Vector2(cell.col * cellSize.x, cell.row * cellSize.y);
                            //pos.y = (cellSize.y + spacing.y) * cell.row - spacing.y + padding.top;
                            cell.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, pos.y, cellSize.y);
                            cell.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pos.x, cellSize.x);
                        }
                        else if(horizontal)
                        {
                            //Vector2 pos = new Vector2(cell.row * cellSize.x, cell.col * cellSize.y);
                            Vector2 pos = (cellSize + spacing) * new Vector2(cell.row, cell.col) - spacing + new Vector2(padding.left, padding.top);
                            cell.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, pos.y, cellSize.y);
                            cell.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pos.x, cellSize.x);
                        }
                    }
                }
                else
                {
                    //not show.position set far away
                    cell.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10000000, cellSize.y);
                    cell.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 10000000, cellSize.x);
                }
            }
        }

        protected void LayoutCells(Vector2 delta)
        {
            //Debug.LogFormat(delta.ToString());
            //计算移动方向
            int moveDirection = 0;
            if (vertical)
            {
                moveDirection = delta.y > 0 ? 1 : (delta.y < 0 ? -1 : 0);
            }
            else if (horizontal)
            {
                moveDirection = delta.x > 0 ? -1 : (delta.x < 0 ? 1 : 0);
            }

            //计算距离
            float distance = 0;
            Cell cell = null;

            if (moveDirection > 0)
            {
                //提前做个判断
                //Debug.LogFormat("{0},{1}", m_Cells.Last.Value.index , m_DataProvider.count - 1);
                if (m_Cells.Last.Value.index >= m_DataProvider.count - 1)
                {
                    //已经到最后一个元素，不能再移动。
                    return;
                }

                cell = m_Cells.First.Value;
                Bounds viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);

                Bounds itemBounds = GetItemBounds(cell.content);

                if (vertical)
                {
                    distance = itemBounds.max.y - viewBounds.max.y;
                }
                else if (horizontal)
                {
                    distance = viewBounds.min.x - itemBounds.min.x;
                }
            }
            else if (moveDirection < 0)
            {
                if (m_Cells.First.Value.index <= 0)
                {
                    //已经到第一个元素，不能在移动。
                    return;
                }

                cell = m_Cells.Last.Value;
                Bounds viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                Bounds itemBounds = GetItemBounds(cell.content);

                if (vertical)
                {
                    distance = viewBounds.min.y - itemBounds.min.y;
                }
                else if (horizontal)
                {
                    distance = itemBounds.max.x - viewBounds.max.x;
                }
            }

            //Debug.Log("d:"+distance);

            if (distance > 0)
            {
                //check is need change
                float rate = distance / cellSize[m_RowAxis];
                float safeThreshold = safeRow + m_Threshold;
                //Debug.Log("rate:" + rate);
                bool needMove = rate >= safeThreshold;
                if (needMove)
                {
                    while (rate >= safeThreshold)
                    {
                        //move to another side
                        if (moveDirection > 0)
                        {
                            //把同一row的元素都移走。
                            var iter = m_Cells.First;
                            var currentRow = iter.Value.row;
                            while (iter.Value.row == currentRow)
                            {
                                //检查是不是到最后一个元素
                                if (m_Cells.Last.Value.index < m_DataProvider.count - 1)
                                {
                                    m_Cells.RemoveFirst();
                                    if (m_Cells.Last.Value.col < m_VisibleColumn - 1)
                                    {
                                        iter.Value.row = m_Cells.Last.Value.row;
                                        iter.Value.col = m_Cells.Last.Value.col + 1;
                                    }
                                    else
                                    {
                                        iter.Value.row = m_Cells.Last.Value.row + 1;
                                        iter.Value.col = 0;
                                    }
                                    iter.Value.index = m_Cells.Last.Value.index + 1;

                                    m_Cells.AddLast(iter);
                                    iter = m_Cells.First;
                                }
                                else
                                {
                                    //提前结束
                                    UpdateCells();
                                    return;
                                }
                            }
                        }
                        else if (moveDirection < 0)
                        {
                            //把同一row的元素都移走。
                            var iter = m_Cells.Last;
                            var currentRow = iter.Value.row;
                            while (iter.Value.row == currentRow)
                            {
                                //检查是不是到第一个元素
                                if (m_Cells.First.Value.index > 0)
                                {
                                    m_Cells.RemoveLast();
                                    if (m_Cells.First.Value.col > 0)
                                    {
                                        iter.Value.row = m_Cells.First.Value.row;
                                        iter.Value.col = m_Cells.First.Value.col - 1;
                                    }
                                    else
                                    {
                                        iter.Value.row = m_Cells.First.Value.row - 1;
                                        iter.Value.col = m_VisibleColumn - 1;
                                    }
                                    iter.Value.index = m_Cells.First.Value.index - 1;
                                    m_Cells.AddFirst(iter);
                                    iter = m_Cells.Last;
                                }
                                else
                                {
                                    //提前结束
                                    UpdateCells();
                                    return;
                                }
                            }
                        }
                        rate -= 1;
                    }
                    UpdateCells();
                }
            }

            //Type t = this.GetType().BaseType;
            //var memInfo = t.GetField("m_ViewBounds", BindingFlags.Instance | BindingFlags.NonPublic);
            //Bounds ViewBounds = (Bounds)memInfo.GetValue(this);
            //Debug.LogFormat("d:{0},{1},cotent:{2},{3},view:{4},{5}",
            //            0,0,
            //            m_ContentBounds.min.y, m_ContentBounds.max.y,
            //            ViewBounds.min.y, ViewBounds.max.y
            // );
        }

        public int GetFirstVisibleIndex()
        {
            Bounds viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            foreach (var item in m_Cells)
            {
                Bounds itemBounds = GetItemBounds(item.content);
                if (vertical)
                {
                    if (itemBounds.min.y <= viewBounds.max.y && itemBounds.max.y > viewBounds.max.y)
                    {
                        return item.index;
                    }
                }
                else
                {
                    if (itemBounds.min.x <= viewBounds.min.x && itemBounds.max.x > viewBounds.min.x)
                    {
                        return item.index;
                    }
                }
            }
            return 0;
        }

        #endregion
    }
}
