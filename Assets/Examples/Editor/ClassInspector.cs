using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SomeComponent))]
public class ClassInspector : Editor
{
    private SerializedObject m_object;

    public void OnEnable()
    {
        m_object = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        m_object.Update();

        GUILayout.Label("Some label", EditorStyles.boldLabel);

        var prop = m_object.FindProperty("SomeColor");
        EditorGUILayout.PropertyField(prop,true);

        prop = m_object.FindProperty("SomeScore");
        EditorGUILayout.PropertyField(prop, true);

        m_object.ApplyModifiedProperties();
    }
}