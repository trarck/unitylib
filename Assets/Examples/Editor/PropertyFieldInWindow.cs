using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace YH
{
    public class PropertyFieldInWindow : EditorWindow
    {
        SerializedObject m_MyObject;
        SerializedProperty m_MyProp;

        [MenuItem("Examples/Test PropertyField")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(PropertyFieldInWindow));
        }

        void OnEnable()
        {
            MyObject obj = ScriptableObject.CreateInstance<MyObject>();
            m_MyObject = new SerializedObject(obj);

            m_MyProp = m_MyObject.FindProperty("myArr");

        }

        void OnGUI()
        {

            m_MyObject.Update();

            EditorGUILayout.PropertyField(m_MyProp, true);

            if (GUILayout.Button("do"))
            {
                DoTest();
            }
            m_MyObject.ApplyModifiedProperties();
        }

        void DoTest()
        {
            MyObject obj=m_MyObject.targetObject as MyObject;

            Debug.Log("value:" + obj.myArr);

            if (obj.myArr!=null)
            {
                for (int i = 0; i < obj.myArr.Length; ++i)
                {
                    Debug.Log(obj.myArr[i]);
                }
            }            
        }
    }
}