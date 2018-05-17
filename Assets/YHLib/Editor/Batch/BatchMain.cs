using UnityEngine;
using UnityEditor;

namespace YH
{

    public class BatchMain : EditorTabs
    {
        [SerializeField]
        FindTab m_FindTab;

        [MenuItem("Window/AssetBatch", priority = 2050)]
        static void ShowWindow()
        {
            var window = GetWindow<BatchMain>();
            window.titleContent = new GUIContent("AssetBatch");
            window.Init();
            window.Show();
        }

        public override void Init()
        {
            base.Init();

            FindTab findTab = new FindTab();
            findTab.name = "Find";
            findTab.Init(this);
            AddTab(findTab);
        }
    }

}