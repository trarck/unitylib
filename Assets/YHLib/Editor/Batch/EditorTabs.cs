using UnityEngine;
using UnityEditor;

namespace YH
{

    public interface IEditorTab
    {
        void Init(EditorWindow owner);
        void Update();
        void Render(Rect pos);

    }

    public class EditorTabs : EditorWindow
    {
        const float k_ToolbarPadding = 15;
        const float k_MenubarPadding = 32;

        IEditorTab[] m_Tabs;
        IEditorTab m_CurrentTab;

        [MenuItem("Window/AssetBatch", priority = 2050)]
        static void ShowWindow()
        {
            var window = GetWindow<BatchMain>();
            window.titleContent = new GUIContent("AssetBatch");
            window.Show();
        }

        void RenderHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(k_ToolbarPadding);
            bool clicked = false;
            //switch (m_Mode)
            //{
            //    case Mode.Browser:
            //        clicked = GUILayout.Button(m_RefreshTexture);
            //        if (clicked)
            //            m_ManageTab.ForceReloadData();
            //        break;
            //    case Mode.Builder:
            //        GUILayout.Space(m_RefreshTexture.width + k_ToolbarPadding);
            //        break;
            //    case Mode.Inspect:
            //        clicked = GUILayout.Button(m_RefreshTexture);
            //        if (clicked)
            //            m_InspectTab.RefreshBundles();
            //        break;
            //}
        }
    }

}