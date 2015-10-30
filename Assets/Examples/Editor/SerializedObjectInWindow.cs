using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace YH
{
    public class SerializedObjectInWindow : EditorWindow
    {
        SerializedObject m_MyObject;

        [MenuItem("Examples/Test SerializedObject")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(SerializedObjectInWindow));
        }

        void OnEnable()
        {
            MyObject obj = ScriptableObject.CreateInstance<MyObject>();
            m_MyObject = new SerializedObject(obj);
        }

        void OnGUI()
        {

            m_MyObject.Update();

            SerializedProperty prop = m_MyObject.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.depth != 0)
                    continue;

                if (prop.name.EndsWith("Script"))
                    continue;

                EditorGUILayout.PropertyField(prop, true);
            }

            if (GUILayout.Button("button"))
            {
                DoTest();
            }

            m_MyObject.ApplyModifiedProperties();
        }

        void DoTest()
        {
            MyObject obj=m_MyObject.targetObject as MyObject;

            Debug.Log("obj props:");

            Debug.LogFormat("myInt={0}", obj.myInt);

            Debug.LogFormat("myArr[{0}]-->", obj.myArr != null?obj.myArr.Length:0);

            if (obj.myArr!=null)
            {
                for (int i = 0; i < obj.myArr.Length; ++i)
                {
                    Debug.LogFormat("myArr value[{0}] :{1}",i,obj.myArr[i]);
                }
            }

            Debug.LogFormat("subObj={0}", obj.subObj);
            if (obj.subObj != null)
            {
                Debug.LogFormat("subObj.To={0}", obj.subObj.To);
            }

            if (obj.subObj.paths != null)
            {
                for (int i = 0; i < obj.subObj.paths.Length; ++i)
                {
                    Debug.LogFormat("subObj  paths[{0}] :{1}", i, obj.subObj.paths[i]);
                }
            }
        }
    }
}