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
    public class BigList:ScrollRect
    {
        public class Item
        {
            public int index;
            public int slot;
            public RectTransform content;
            public int lastIndex = -1;
        }

        public interface IDataProvider
        {
            //总数量
            int count { get; }
            //元素大小
            float itemSize { get; }
            //开始元素号
            int startIndex { get; }
            //创建元素
            Item CreateItem(int index, BigList list);
            //更新元素
            void UpdateItem(Item item);
        }

        protected IDataProvider m_DataProvider=null;
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
                    InitItems();
                }
            }
        }

        ////总的个数
        //public int count=0;
        ////item的高或宽。根据显示方式来定
        //public float itemSize;

        //安全个数。前后各多创建的个数
        public int safeCount=2;
        //当前显示的个数。
        protected int m_ShowCount = 0;
        //使用的数量TotalCount=ShowCount+SafeCount*2;
        protected int useCount
        {
            get
            {
                return m_ShowCount + safeCount * 2;
            }
        }

        //移动方向。1:正向，-1:负向,0:方向
        protected int m_MoveDirection = 0;
        //改变阀值。不能设置为0.5，否则可能会发生抖动。
        protected float m_Threshold = 0.6f;
        
        LinkedList<Item> m_Items;

        protected void InitItems()
        {
            m_Items = new LinkedList<Item>();
            CalculateShowCount();
            CreateItems();
            UpdateItems();
            //set content size
            content.SetSizeWithCurrentAnchors(vertical ? RectTransform.Axis.Vertical : RectTransform.Axis.Horizontal, m_DataProvider.itemSize * m_DataProvider.count);
        }

        protected void CalculateShowCount()
        {
            //计算要显示的元素个数
            if (vertical)
            {
                m_ShowCount = Mathf.CeilToInt(viewRect.rect.height / m_DataProvider.itemSize);
            }
            else
            {
                m_ShowCount = Mathf.CeilToInt(viewRect.rect.width / m_DataProvider.itemSize);
            }
        }

        public override void OnScroll(PointerEventData data)
        {
            Vector2 oldPosition = content.anchoredPosition;
            base.OnScroll(data);
            Vector2 delta = content.anchoredPosition - oldPosition;
            LayoutItems(delta);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            Vector2 oldPosition = content.anchoredPosition;
            base.OnDrag(eventData);
            Vector2 delta = content.anchoredPosition - oldPosition;
            LayoutItems(delta);
        }

        protected override void LateUpdate()
        {
            Vector2 oldPosition = content.anchoredPosition;
            base.LateUpdate();
            Vector2 delta = content.anchoredPosition - oldPosition;
#if UNITY_EDITOR
            if (m_DataProvider == null)
            {
                return;
            }
#endif
            LayoutItems(delta);
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
       
        protected Item CreateItem(int index, int slot)
        {
            Item item = null;
            if (m_DataProvider != null)
            {
                item = m_DataProvider.CreateItem(index,this);
            }
            else
            {
                item = new Item();

                GameObject itemObj = new GameObject();
                item.content = itemObj.AddComponent<RectTransform>(); 
            }

            if (item!=null)
            {
                item.index = index;
                item.slot = slot;
                if (item.content!=null && item.content.parent != content)
                {
                    item.content.SetParent(content);
                }
            }

            return item;
        }

        protected void CreateItems()
        {
            int slotIndex = 0;
            int startIdnex = m_DataProvider.startIndex;
            int beforeCount = Mathf.Min(safeCount, startIdnex);
            int afterCount = safeCount - beforeCount + safeCount;
            //before
            for (int i = 0; i < beforeCount; ++i)
            {
                Item  item = CreateItem(startIdnex - beforeCount + i, slotIndex++);
                m_Items.AddLast(item);
            }
            //display
            for (int i = 0; i < m_ShowCount; ++i)
            {
                Item item = CreateItem(startIdnex + i, slotIndex++);
                m_Items.AddLast(item);
            }
            //after
            for (int i = 0; i < afterCount; ++i)
            {
                Item item = CreateItem(startIdnex + m_ShowCount + i, slotIndex++);
                m_Items.AddLast(item);
            }

            
        }

        protected void UpdateItems()
        {
            float p = 0;
            foreach(Item item in m_Items)
            {
                if(item.index>=0 &&  item.index< m_DataProvider.count)
                {
                    if (item.index != item.lastIndex)
                    {
                        if (m_DataProvider != null)
                        {
                            m_DataProvider.UpdateItem(item);
                            item.lastIndex = item.index;
                        }

                        //update position
                        p = item.index * m_DataProvider.itemSize;

                        item.content.SetInsetAndSizeFromParentEdge(vertical?RectTransform.Edge.Top:RectTransform.Edge.Left, p, m_DataProvider.itemSize);
                    }
                }
                else
                {
                    //not show.position set far away
                    item.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -10000, m_DataProvider.itemSize);
                }
            }
        }

        protected void LayoutItems(Vector2 delta)
        {
            //Debug.LogFormat(delta.ToString());
            //计算移动方向
            int moveDirection = 0;
            if (vertical)
            {
                moveDirection = delta.y > 0 ? 1 : (delta.y < 0 ? -1 : 0);
            }
            else if(horizontal)
            {
                moveDirection = delta.x > 0 ? -1 : (delta.x < 0 ? 1 : 0);
            }

            //计算距离
            float distance = 0;
            Item item = null;

            if (moveDirection > 0)
            {
                //提前做个判断
                if (m_Items.Last.Value.index >= m_DataProvider.count - 1)
                {
                    //已经到最后一个元素，不能再移动。
                    return;
                }

                item = m_Items.First.Value;
                Bounds viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);

                Bounds itemBounds = GetItemBounds(item.content);

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
                if (m_Items.First.Value.index <= 0)
                {
                    //已经到第一个元素，不能在移动。
                    return;
                }

                item = m_Items.Last.Value;
                Bounds viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                Bounds itemBounds = GetItemBounds(item.content);

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
                float rate = distance / m_DataProvider.itemSize;
                float safeThreshold = safeCount + m_Threshold;
                bool needMove = rate >= safeThreshold;
                if (needMove)
                {
                    LinkedListNode<Item> iter = null;
                    while (rate >= safeThreshold)
                    {
                        //move to another side
                        if (moveDirection > 0)
                        {
                            //检查是不是到最后一个元素
                            if (m_Items.Last.Value.index < m_DataProvider.count - 1)
                            {
                                iter = m_Items.First;
                                m_Items.RemoveFirst();
                                iter.Value.index = m_Items.Last.Value.index + 1;
                                m_Items.AddLast(iter);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (moveDirection < 0)
                        {
                            //检查是不是到第一个元素
                            if (m_Items.First.Value.index > 0)
                            {
                                iter = m_Items.Last;
                                m_Items.RemoveLast();
                                iter.Value.index = m_Items.First.Value.index - 1;
                                m_Items.AddFirst(item);
                            }
                            else
                            {
                                break;
                            }
                        }
                        rate -= 1;
                    }
                    UpdateItems();
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
            foreach (var item in m_Items)
            {
                Bounds itemBounds = GetItemBounds(item.content);
                if (vertical)
                {
                    if(itemBounds.min.y <= viewBounds.max.y && itemBounds.max.y > viewBounds.max.y)
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
