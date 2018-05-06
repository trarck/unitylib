using UnityEngine;
using UnityEditor;

namespace YH
{

    public class BatchMain : EditorWindow
    {
        [SerializeField]
        FindTab m_FindTab;

        [MenuItem("Window/AssetBatch", priority = 2050)]
        static void ShowWindow()
        {
            var window = GetWindow<BatchMain>();
            window.titleContent = new GUIContent("AssetBatch");
            window.Show();
        }
    }

}