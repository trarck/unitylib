using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH;

namespace UI
{
    public class UIDirector : IDirector
    {
        public class StackInfo
        {
            //public string asset;
            public UIPanel panel;
            public object data;

            public StackInfo(UIPanel panel, object data)
            {
                this.panel = panel;
                this.data = data;
            }
        }

        //panel的记录。为了性能只记录panel的资源路径。
        Stack<StackInfo> m_Stack = new Stack<StackInfo>();

        UIManager m_Manager = null;

        List<StackInfo> m_TempInfos;

        public UIDirector(UIManager manage)
        {
            m_Manager = manage;
        }

        public void Push(string panelAsset, object data = null)
        {
            m_Manager.LoadPanel(panelAsset, (panel) =>
            {
                if (panel != null)
                {
                    panel.Init(data);

                    panel.Show();

                    m_Manager.AddPanel(panel);

                    //add to stack
                    if (m_Stack.Count > 0)
                    {
                        StackInfo old = m_Stack.Peek();

                        if (old.panel != panel)
                        {
                            old.panel.Hide();
                        }
                    }

                    m_Stack.Push(new StackInfo(panel, data));
                }
            }, m_Manager.root);
        }

        public void Pop()
        {
            if (m_Stack.Count == 1)
            {
                Debug.Log("At root can't pop");
                return;
            }

            StackInfo old = m_Stack.Pop();

            StackInfo info = m_Stack.Peek();
            //先show,后close
            info.panel.Show();

            if (IsUsing(old.panel))
            {
                if (old.panel != info.panel)
                {
                    old.panel.Hide();
                }
            }
            else
            {
                m_Manager.ClosePanel(old.panel);
            }
        }

        public void Replace(string panelAsset, object data = null)
        {
            StackInfo old = null;
            if (m_Stack.Count > 0)
            {
                old = m_Stack.Pop();
            }


            m_Manager.LoadPanel(panelAsset, (panel) =>
            {
                if (panel != null)
                {
                    panel.Init(data);

                    panel.Show();

                    m_Manager.AddPanel(panel);

                    if (old != null)
                    {
                        if (IsUsing(old.panel))
                        {
                            if (old.panel != panel)
                            {
                                old.panel.Hide();
                            }
                        }
                        else
                        {
                            m_Manager.ClosePanel(old.panel);
                        }
                    }

                    m_Stack.Push(new StackInfo(panel, data));
                }
            }, m_Manager.root);

        }

        public void PopToStackLevel(int level)
        {
            if (level == 0)
            {
                //do nothing
                return;
            }
            else if (level < 0)
            {
                //remove tops
                level = m_Stack.Count + level;
            }

            int c = m_Stack.Count;

            if (m_TempInfos == null)
            {
                m_TempInfos = new List<StackInfo>();
            }

            while (c-- > level)
            {
                m_TempInfos.Add(m_Stack.Pop());
            }

            StackInfo info = m_Stack.Peek();
            //先show，后remove
            info.panel.Show();

            for (int i = 0; i < m_TempInfos.Count; ++i)
            {
                info = m_TempInfos[i];
                if (!IsUsing(info.panel))
                {
                    m_Manager.ClosePanel(info.panel);
                }
            }
            m_TempInfos.Clear();
        }

        public void PopToRoot()
        {
            PopToStackLevel(1);
        }

        bool IsUsing(UIPanel panel)
        {
            foreach (var iter in m_Stack)
            {
                if (iter.panel == panel)
                {
                    return true;
                }
            }

            return false;
        }

        public UIPanel active
        {
            get
            {
                return m_Stack.Peek().panel;
            }
        }
    }
}