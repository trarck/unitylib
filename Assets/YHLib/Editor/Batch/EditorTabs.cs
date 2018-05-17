using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YH
{
    public interface IEditorTab
    {
        void Init(EditorWindow owner);
        void Update(float delta);
        void OnGUI(Rect pos);

        string name { get; set; }
    }

    public class EditorTabs : EditorWindow
    {
        const float k_ToolbarPadding = 15;
        const float k_MenubarPadding = 32;

        List<IEditorTab> m_Tabs=new List<IEditorTab>();
        IEditorTab m_CurrentTab;
        int m_SelectIndex=0;

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
            m_SelectIndex = GUILayout.Toolbar(m_SelectIndex, GetTabNames(), "LargeButton", GUILayout.Width(toolbarWidth));
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