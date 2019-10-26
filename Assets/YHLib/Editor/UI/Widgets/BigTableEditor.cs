using UnityEditor;
using UnityEditor.UI;
using YH.UI;

namespace YHEditor.UI.Widgets
{
    [CustomEditor(typeof(BigTable), true)]
    [CanEditMultipleObjects]
    public class BigTableEditor : ScrollRectEditor
    {
        SerializedProperty m_SafeRow;
        SerializedProperty m_Padding;
        SerializedProperty m_Spacing;
        SerializedProperty m_CellSize;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_SafeRow = serializedObject.FindProperty("safeRow");
            m_Padding = serializedObject.FindProperty("padding");
            m_Spacing = serializedObject.FindProperty("spacing");
            m_CellSize = serializedObject.FindProperty("cellSize");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_SafeRow);
            EditorGUILayout.PropertyField(m_Padding,true);
            EditorGUILayout.PropertyField(m_Spacing);
            EditorGUILayout.PropertyField(m_CellSize);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
