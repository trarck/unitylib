using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH;

namespace YH.UI
{
    public class UIDirector : IDirector
    {
        //panel的记录。为了性能只记录panel的资源路径。
        Stack<PanelInfo> m_Stack = new Stack<PanelInfo>();

        UIManager m_Manager = null;

        List<PanelInfo> m_TempInfos;

        public UIDirector(UIManager manage)
        {
            m_Manager = manage;
        }
        
        void HidePanel(PanelInfo panelStack)
        {
            if (panelStack.IsLoaded)
            {
                if (panelStack.panel)
                {
                    panelStack.panel.Hide();
                }
            }
            else
            {
                panelStack.visible = false;
            }
        }

        void ClosePanel(PanelInfo panelStack)
        {
            if (panelStack.IsLoaded)
            {
                if (panelStack.panel)
                {
                    panelStack.panel.Close();
                }
            }
            else
            {
                panelStack.closble = true;
            }
        }

        void LoadPanel(PanelInfo panelStack, Transform parent,System.Action callback)
        {
            panelStack.state = PanelInfo.State.Loading;
            m_Manager.LoadPanel(panelStack.asset, (panel) =>
            {
                panelStack.state = PanelInfo.State.Loaded;

                if (panel != null)
                {
                    panelStack.panel = panel;

                    if (panelStack.closble)
                    {
                        m_Manager.ClosePanel(panel);
                    }
                    else
                    {
                        panel.Init(panelStack.data);
                        m_Manager.AddPanel(panel);
                        if (panelStack.visible)
                        {
                            panel.Show();
                        }
                        else
                        {
                            m_Manager.HidePanel(panel);
                        }
                    }

                    if (callback != null)
                    {
                        callback();
                    }
                }
            }, parent);
        }


        public void Push(string panelAsset, object data = null)
        {
            PanelInfo old = null;
            if (m_Stack.Count > 0)
            {
                old = m_Stack.Peek();
            }
            
            PanelInfo current = new PanelInfo(panelAsset,data);
            LoadPanel(current, m_Manager.root,() =>
            {
                if (old!=null)
                {
                    if (old.asset != current.asset)
                    {
                        HidePanel(old);
                    }
                }
            });
            m_Stack.Push(current);
        }

        public void Pop()
        {
            if (m_Stack.Count == 1)
            {
                Debug.Log("At root can't pop");
                return;
            }

            PanelInfo old = m_Stack.Pop();

            PanelInfo info = m_Stack.Peek();
            //先show,后close
            if (info.IsLoaded)
            {
                //已经加载过。则直接显示。否则会在加载完成后默认显示。
                info.panel.Show();
            }

            //检查在不在使用
            if (IsUsing(old.asset))
            {
                if (old.asset != info.asset)
                {
                    HidePanel(old);
                }
            }
            else
            {
                ClosePanel(old);
            }
        }

        public void Replace(string panelAsset, object data = null)
        {
            PanelInfo old = null;
            if (m_Stack.Count > 0)
            {
                old = m_Stack.Pop();
            }

            PanelInfo current = new PanelInfo(panelAsset, data);
            LoadPanel(current, m_Manager.root, () => {
                if (old != null)
                {
                    if (IsUsing(old.asset))
                    {
                        if (old.asset != current.asset)
                        {
                            HidePanel(old);
                        }
                    }
                    else
                    {
                        ClosePanel(old);
                    }
                }
            });

            m_Stack.Push(current);
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
                m_TempInfos = new List<PanelInfo>();
            }

            while (c-- > level)
            {
                m_TempInfos.Add(m_Stack.Pop());
            }

            PanelInfo info = m_Stack.Peek();
            //先show，后remove
            if (info.IsLoaded)
            {
                info.panel.Show();
            }

            for (int i = 0; i < m_TempInfos.Count; ++i)
            {
                info = m_TempInfos[i];
                if (!IsUsing(info.asset))
                {
                    ClosePanel(info);
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

        bool IsUsing(string panelAsset)
        {
            foreach (var iter in m_Stack)
            {
                if (iter.asset == panelAsset)
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