using UnityEngine;
using System.Collections;
using UnityEditor;

namespace YH
{
    public class MyWindow : EditorWindow
    {

        string m_Path = "";
        AnimationClip clip;

        // Add menu item named "My Window" to the Window menu
        [MenuItem("MyMenu/My Window")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(MyWindow));
        }

        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            clip = EditorGUILayout.ObjectField("Clip", clip, typeof(AnimationClip), false) as AnimationClip;

            m_Path = EditorGUILayout.TextField("path", m_Path);

            if (GUILayout.Button("click"))
            {
                TestFind(m_Path);


            }
        }

        void TestFind(string path)
        {
            Debug.Log("look for " + path);
            GameObject root = Selection.activeGameObject;
            GameObject obj = FindUtil.SearchGameObject(path, root);
            Debug.Log("searched:" + obj);

            if (obj)
            {
                EditorCurveBinding[] bindings = AnimationUtility.GetAnimatableBindings(obj, root);
                for (int i = 0; i < bindings.Length; ++i)
                {
                    EditorCurveBinding binding = bindings[i];
                    Debug.Log("binding path=" + binding.path + ",propName=" + binding.propertyName + ",PPtrCurve=" + binding.isPPtrCurve + ",type=" + binding.type);
                }
            }
        }
    }
}