using UnityEditor;
using UnityEditor.UI;
using YH.UI;

namespace YHEditor.UI.Widgets
{
    [CustomEditor(typeof(BigList), true)]
    [CanEditMultipleObjects]
    public class BigListEditor:ScrollRectEditor
    {
        SerializedProperty m_SafeCount;
        SerializedProperty m_ItemSize;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_SafeCount = serializedObject.FindProperty("safeCount");
            m_ItemSize = serializedObject.FindProperty("itemSize");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_SafeCount);
            EditorGUILayout.PropertyField(m_ItemSize);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
