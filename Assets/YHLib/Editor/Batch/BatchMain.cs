using UnityEngine;
using UnityEditor;
using YH;

namespace YHEditor
{

    public class BatchMain : EditorTabs
    {
        [SerializeField]
        FindTab m_FindTab;

        Batch m_Batch;

        public Batch controller
        {
            get
            {
                return m_Batch;
            }
        }

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

            m_Batch = new Batch();
            m_Batch.Init();

            FindTab findTab = new FindTab();
            findTab.name = "Find";
            findTab.Init(this);
            AddTab(findTab);


            ResultTab resultTab = new ResultTab();
            resultTab.name = "Result";
            resultTab.Init(this);
            AddTab(resultTab);

            ModifyTab modifyTab = new ModifyTab();
            modifyTab.name = "Modify";
            modifyTab.Init(this);
            AddTab(modifyTab);
        }

    }

}