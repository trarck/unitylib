using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

namespace YH
{
    public class FindAssetWindow : EditorWindow
    {

        string m_Path = "";
        string m_Filter = "";
        Object m_Obj;

        Vector2 mScroll = Vector2.zero;

        // Add menu item named "My Window" to the Window menu
        [MenuItem("Assets/FindAsset")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(FindAssetWindow));
        }

        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            m_Obj = EditorGUILayout.ObjectField("For:", m_Obj, typeof(Object), false) as Object;

            m_Path = EditorGUILayout.TextField("path", m_Path);

            if (GUILayout.Button("click"))
            {
               List<string> result=FindAllAssetsByType(m_Obj.GetType(), m_Path,m_Filter);
               if(result!=null && result.Count > 0)
                {

                }
            }
        }

        void ShowFindResult(Dictionary<string, List<string>> result)
        {
            if (result == null)
            {
                return;
            }
            mScroll = GUILayout.BeginScrollView(mScroll);

            List<string> list = result["prefab"];
            if (list != null && list.Count > 0)
            {
                if (YHEditorTools.DrawHeader("Prefab"))
                {
                    foreach (string item in list)
                    {
                        GameObject go = AssetDatabase.LoadAssetAtPath(item, typeof(GameObject)) as GameObject;
                        EditorGUILayout.ObjectField("Prefab", go, typeof(GameObject), false);

                    }
                }
                list = null;
            }

            list = result["fbx"];
            if (list != null && list.Count > 0)
            {
                if (YHEditorTools.DrawHeader("FBX"))
                {
                    foreach (string item in list)
                    {
                        GameObject go = AssetDatabase.LoadAssetAtPath(item, typeof(GameObject)) as GameObject;
                        EditorGUILayout.ObjectField("FBX", go, typeof(GameObject), false);

                    }
                }
                list = null;
            }

            list = result["cs"];
            if (list != null && list.Count > 0)
            {
                if (YHEditorTools.DrawHeader("Script"))
                {
                    foreach (string item in list)
                    {
                        MonoScript go = AssetDatabase.LoadAssetAtPath(item, typeof(MonoScript)) as MonoScript;
                        EditorGUILayout.ObjectField("Script", go, typeof(MonoScript), false);

                    }
                }
                list = null;
            }

            list = result["texture"];
            if (list != null && list.Count > 0)
            {
                if (YHEditorTools.DrawHeader("Texture"))
                {
                    foreach (string item in list)
                    {
                        Texture2D go = AssetDatabase.LoadAssetAtPath(item, typeof(Texture2D)) as Texture2D;
                        EditorGUILayout.ObjectField("Texture:" + go.name, go, typeof(Texture2D), false);

                    }
                }
                list = null;
            }

            list = result["mat"];
            if (list != null && list.Count > 0)
            {
                if (YHEditorTools.DrawHeader("Material"))
                {
                    foreach (string item in list)
                    {
                        Material go = AssetDatabase.LoadAssetAtPath(item, typeof(Material)) as Material;
                        EditorGUILayout.ObjectField("Material", go, typeof(Material), false);

                    }
                }
                list = null;
            }

            list = result["shader"];
            if (list != null && list.Count > 0)
            {
                if (YHEditorTools.DrawHeader("Shader"))
                {
                    foreach (string item in list)
                    {
                        Shader go = AssetDatabase.LoadAssetAtPath(item, typeof(Shader)) as Shader;
                        EditorGUILayout.ObjectField("Shader", go, typeof(Shader), false);
                    }
                }
                list = null;
            }

            list = result["font"];
            if (list != null && list.Count > 0)
            {
                if (YHEditorTools.DrawHeader("Font"))
                {
                    foreach (string item in list)
                    {
                        Font go = AssetDatabase.LoadAssetAtPath(item, typeof(Font)) as Font;
                        EditorGUILayout.ObjectField("Font", go, typeof(Font), false);
                    }
                }
                list = null;
            }

            list = result["anim"];
            if (list != null && list.Count > 0)
            {
                if (YHEditorTools.DrawHeader("Animation"))
                {
                    foreach (string item in list)
                    {
                        AnimationClip go = AssetDatabase.LoadAssetAtPath(item, typeof(AnimationClip)) as AnimationClip;
                        EditorGUILayout.ObjectField("Animation:", go, typeof(AnimationClip), false);
                    }
                }
                list = null;
            }

            list = result["animTor"];
            if (list != null && list.Count > 0)
            {
                if (YHEditorTools.DrawHeader("Animator"))
                {
                    foreach (string item in list)
                    {
                        //Animator go = AssetDatabase.LoadAssetAtPath(item, typeof(Animator)) as Animator;
                        //EditorGUILayout.ObjectField("Animator:", go, typeof(Animator), true);
                        EditorGUILayout.LabelField(item);
                    }
                }
                list = null;
            }

            list = result["level"];
            if (list != null && list.Count > 0)
            {
                if (YHEditorTools.DrawHeader("Level"))
                {
                    foreach (string item in list)
                    {
                        //SceneView go = AssetDatabase.LoadAssetAtPath(item, typeof(SceneView)) as SceneView;
                        EditorGUILayout.LabelField(item);
                        //SceneView go = AssetDatabase.LoadAssetAtPath(item, typeof(SceneView)) as SceneView;
                        //EditorGUILayout.ObjectField("Animation:" , go, typeof(SceneView), true);
                    }
                }
                list = null;
            }

            GUILayout.EndScrollView();
        }

        void ShowFindResult(List<string> result)
        {
            if (YHEditorTools.DrawHeader("Result:"))
            {
                foreach (string item in result)
                {
                    Object obj = AssetDatabase.LoadAssetAtPath(item,typeof(Object));
                    EditorGUILayout.ObjectField("Object",obj, typeof(Object), false);
                }
            }
        }

        static List<string> FindAllAssetsByType(Type type,string path,string filter)
        {
            List<string> result = new List<string>();

            string[] allGuids = AssetDatabase.FindAssets(filter, new string[] { path });
            for(int i = 0; i < allGuids.Length; ++i)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allGuids[i]);
                Object obj = AssetDatabase.LoadAssetAtPath(assetPath, type);
                if (obj != null)
                {
                    result.Add(assetPath);
                }
            }

            return result;
        }
    }
}