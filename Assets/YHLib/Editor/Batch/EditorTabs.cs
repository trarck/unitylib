using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using YH;

namespace YHEditor
{
    public interface IEditorTab
    {
        void Init(EditorTabs owner);
        void Update(float delta);
        void OnGUI(Rect pos);

        void OnEnter();
        void OnExit();

        string name { get; set; }
    }

    [System.Serializable]
    public class EditorTabs : EditorWindow
    {
        const float k_ToolbarPadding = 15;
        const float k_MenubarPadding = 32;

        [SerializeField]
        List<IEditorTab> m_Tabs=new List<IEditorTab>();

        [SerializeField]
        int m_SelectIndex=0;

        protected bool m_Inited=false;
        //[MenuItem("Window/EditorTabTest", priority = 2050)]
        //static void ShowWindow()
        //{
        //    var window = GetWindow<EditorTabs>();
        //    window.titleContent = new GUIContent("TestEditorTabs");
        //    window.Init();
        //    window.Show();
        //}

        public virtual void Init()
        {
            if (!m_Inited)
            {
                m_Inited = true;
            }
        }

        void OnGUI()
        {
            RenderHeader();
            if (m_Tabs != null && m_Tabs.Count>0)
            {
                m_Tabs[m_SelectIndex].OnGUI(GetSubWindowArea());
            }
        }

        void RenderHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(k_ToolbarPadding);

            float toolbarWidth = position.width - k_ToolbarPadding * 4;
            int index = GUILayout.Toolbar(m_SelectIndex, GetTabNames(), "LargeButton", GUILayout.Width(toolbarWidth));
            ChangeTab(index);

            GUILayout.EndHorizontal();
        }

        #region Tab
        public void AddTab(IEditorTab tab)
        {
            if (!m_Tabs.Contains(tab))
            {
                m_Tabs.Add(tab);
            }
        }

        public void RemoveTab(IEditorTab tab)
        {
            if (m_Tabs.Contains(tab))
            {
                m_Tabs.Remove(tab);
            }
        }

        public void RemoveTab(int index)
        {
            m_Tabs.RemoveAt(index);
        }

        public void ChangeTab(int index)
        {
            if (index > -1 && index<m_Tabs.Count)
            {
                if (m_SelectIndex != index)
                {
                    m_Tabs[m_SelectIndex].OnExit();
                    m_SelectIndex = index;
                    m_Tabs[m_SelectIndex].OnEnter();
                }
            }
        }

        public void ChangeTab(IEditorTab tab)
        {
            int index = m_Tabs.IndexOf(currentTab);
            ChangeTab(index);
        }

        public void ChangeTab(string name)
        {
            for(int i = 0; i < m_Tabs.Count; ++i)
            {
                if (m_Tabs[i].name == name)
                {
                    ChangeTab(i);
                    break;
                }
            }
        }

        public IEditorTab currentTab
        {
            get
            {
                if(m_SelectIndex>=0 && m_SelectIndex < m_Tabs.Count)
                {
                    return m_Tabs[m_SelectIndex];
                }
                return null;
            }
            set
            {
                ChangeTab(value);
            }
        }

        string[] GetTabNames()
        {
            string[] names = null;
            if (m_Tabs != null)
            {
                names = new string[m_Tabs.Count];
                for (int i = 0; i < m_Tabs.Count; ++i)
                {
                    names[i] = m_Tabs[i].name;
                }
            }
            return names;
        }

        #endregion
        private Rect GetSubWindowArea()
        {
            float padding = k_MenubarPadding;
            Rect subPos = new Rect(0, padding, position.width, position.height - padding);
            return subPos;
        }
    }

}