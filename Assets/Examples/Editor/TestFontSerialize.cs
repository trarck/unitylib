using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace YH
{
    public class TestFontSerialize : EditorWindow
    {
        SerializedObject m_MyObject;

        Vector2 m_ScrollViewPos = Vector2.zero;

        [MenuItem("Examples/Test Font")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(TestFontSerialize));
        }

        void OnEnable()
        {
            Font font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Tests/Fonts/arial_copy.fontsettings");
            Debug.Log(font);

            Test(font);

            m_MyObject = new SerializedObject(font);



            //SerializedProperty prop = m_MyObject.FindProperty("m_KerningValues");

            //prop.InsertArrayElementAtIndex(2);

            //m_MyObject.ApplyModifiedProperties();

            ////IDictionary d = (IDictionary)prop;
            //while (prop.Next(true))
            //{
            //    Debug.Log(prop.name + "[" + prop.depth + "]" + "," + prop.type + "," + prop.hasChildren);
            //}

            //Debug.Log(prop.name + "," + prop.type + "," + prop.hasChildren);
            //IEnumerator it = prop.GetEnumerator();
            ////Debug.Log(((SerializedProperty)it.Current).name);
            ////while (it.MoveNext())
            ////{
            ////    //SerializedProperty sp = (SerializedProperty)it.Current;
            ////    //Debug.Log(sp.name+","+sp.type+","+sp.hasChildren);
            ////    while (sp.Next(true))
            ////    {
            ////        Debug.Log(sp.name+"["+sp.depth+"]" + "," + sp.type + "," + sp.hasChildren);
            ////    }
            ////}
        }


        void Test(Font font)
        {
            Editor editor = Editor.CreateEditor(font);

            //lineSpacing
            SerializedProperty kerningValues = editor.serializedObject.FindProperty("m_KerningValues");

            kerningValues.InsertArrayElementAtIndex(0);

            SerializedProperty end;

            SerializedProperty prop = kerningValues.Copy();
            end = prop.GetEndProperty(true);
            Debug.Log("end:" + end.name + "," + end.type);

            prop = kerningValues;

            //array prop
            //prop.Next(true);
            ////size prop;
            //prop.Next(true);

            ////data prop map< pair<ushort,ushort>,float >
            //prop.Next(true);
            ////key <ushort,ushort> first,second
            //prop.Next(true);
            ////key first
            //prop.Next(true);
            //prop.intValue = 49;
            ////key second
            //prop.Next(true);
            //prop.intValue = 49;
            ////second
            //prop.Next(true);
            //prop.floatValue = -2;

            //Debug.Log(prop.name + "[" + prop.depth + "]" + "," + prop.type + "," + prop.hasChildren);

            while (prop.Next(true))
            {
                if (SerializedProperty.EqualContents(prop, end))
                {
                    break;
                }
                Debug.Log(prop.name + "[" + prop.depth + "]" + "," + prop.type + "," + prop.hasChildren);

            }

            editor.serializedObject.ApplyModifiedProperties();
        }

        void OnGUI()
        {
            m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos);

            m_MyObject.Update();
            SerializedProperty prop = m_MyObject.FindProperty("m_KerningValues");

            //SerializedProperty prop = m_MyObject.GetIterator();

            while (prop.Next(true))
            {
                //if (prop.depth != 0)
                //    continue;
                //Debug.Log(prop.name);
                EditorGUILayout.PropertyField(prop, true);
            }

            //EditorGUILayout.PropertyField(prop, true);

            if (GUILayout.Button("button"))
            {
                DoTest();
            }

            m_MyObject.ApplyModifiedProperties();

            EditorGUILayout.EndScrollView();
        }

        void DoTest()
        {
            MyObject obj = m_MyObject.targetObject as MyObject;

            Debug.Log("obj props:");

            Debug.LogFormat("myInt={0}", obj.myInt);

            Debug.LogFormat("myArr[{0}]-->", obj.myArr != null ? obj.myArr.Length : 0);

            if (obj.myArr != null)
            {
                for (int i = 0; i < obj.myArr.Length; ++i)
                {
                    Debug.LogFormat("myArr value[{0}] :{1}", i, obj.myArr[i]);
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