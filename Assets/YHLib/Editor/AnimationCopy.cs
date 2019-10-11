using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using YHEditor;

namespace YH
{
    public class AnimationCopy : EditorWindow
    {
        public enum FixType
        {
            Auto = 1,
            ManualList,
            ManualReplace
        }

        AnimationClip m_Clip;
        GameObject m_Root;

        FixType m_FixType = FixType.Auto;

        //for manual list
        Dictionary<string, ArrayList> m_WrongPathBindings;
        string[] m_ToPaths;

        //for manual replace
        string m_PathFrom = "";
        string m_PathTo = "";

        [MenuItem("Animation/Copy")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(AnimationEditor));
        }

        void OnGUI()
        {
            //GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            m_Clip = EditorGUILayout.ObjectField("Clip", m_Clip, typeof(AnimationClip), false) as AnimationClip;
            m_Root = EditorGUILayout.ObjectField("Root", m_Root, typeof(GameObject), true) as GameObject;

            m_FixType = (FixType)EditorGUILayout.EnumPopup("Fix Type", m_FixType);

            switch (m_FixType)
            {
                case FixType.ManualList:
                    ShowManualListPanel();
                    break;
                case FixType.ManualReplace:
                    ShowManualReplacePanel();
                    break;
            }

            if (GUILayout.Button("fix"))
            {
                switch (m_FixType)
                {
                    case FixType.Auto:
                        DoAutoFix();
                        break;
                    case FixType.ManualList:
                        DoManualListFix();
                        break;
                    case FixType.ManualReplace:
                        DoManualReplaceFix();
                        break;
                }
            }
        }

        void ShowManualListPanel()
        {
            YHEditorTools.DrawSeparator();

            if (GUILayout.Button("load"))
            {
                m_WrongPathBindings = GetWrongPathBindings();
                m_ToPaths = new string[m_WrongPathBindings.Count];
            }

            if (m_WrongPathBindings != null && m_WrongPathBindings.Count > 0)
            {
                ShowWrongPaths();
            }
        }

        void ShowWrongPaths()
        {
            //title
            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("", GUILayout.Width(100));
            EditorGUILayout.LabelField("from");
            EditorGUILayout.LabelField("to");
            EditorGUILayout.EndHorizontal();

            //content
            int i = 0;

            foreach (KeyValuePair<string, ArrayList> it in m_WrongPathBindings)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(it.Key);
                m_ToPaths[i] = EditorGUILayout.TextField(m_ToPaths[i]);
                EditorGUILayout.EndHorizontal();
                ++i;
            }
        }

        void ShowManualReplacePanel()
        {
            m_PathFrom = EditorGUILayout.TextField("from", m_PathFrom);
            m_PathTo = EditorGUILayout.TextField("to", m_PathTo);
        }

        void DoAutoFix()
        {
            if (m_Clip)
            {
                GameObject root = GetRoot();
                if (root)
                {
                    EditorCurveBinding[] bindings = GetAllCurveBindings(m_Clip);
                    for (int i = 0; i < bindings.Length; ++i)
                    {
                        EditorCurveBinding binding = bindings[i];
                        Debug.Log("binding path=" + binding.path + ",propName=" + binding.propertyName + ",PPtrCurve=" + binding.isPPtrCurve + ",type=" + binding.type);

                        GameObject animatedObject = AnimationUtility.GetAnimatedObject(root, binding) as GameObject;

                        if (!animatedObject && !root.transform.Find(binding.path))
                        {
                            //不存在则修正
                            //Debug.Log("can't find:" + binding.path);
                            //二种修正，一种是上移，从子节点移到父结点。一种是下移，从父节点移到子结点。

                            string path = binding.path;

                            //查找存在的父结点。存在的就是没有被移动的。
                            string[] result = LookForExists(root, path);
                            //自动修正，只能修正不会有重名的元素。
                            //比如根结点有个Name，Panel有个Name,把Name移到，PanelTT下，则可能无法正确找到PanelTT下的Name

                            //获取被移动GameObject
                            animatedObject = HierarchyUtil.SearchGameObject(result[1], root);
                            if (!animatedObject)
                            {
                                Debug.LogWarning("can't find:" + result[1]);
                                continue;
                            }

                            string newPath = AnimationUtility.CalculateTransformPath(animatedObject.transform, root.transform);
                            Debug.Log("fix path:" + path + ",to:" + newPath);

                            FixCurveBinding(m_Clip, binding, newPath);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("root is null");
                }
            }
            else
            {
                Debug.LogWarning("AnimationClip is null");
            }
        }

        void DoManualListFix()
        {
            int i = 0;
            string fromPath = "", toPath = "";


            foreach (KeyValuePair<string, ArrayList> it in m_WrongPathBindings)
            {
                fromPath = it.Key;
                toPath = m_ToPaths[i];


                ArrayList bindings = it.Value;

                Debug.Log("fix from:" + fromPath + ",to:" + toPath + "," + bindings.Count);

                for (int j = 0; j < bindings.Count; ++j)
                {
                    EditorCurveBinding binding = (EditorCurveBinding)bindings[j];
                    FixCurveBinding(m_Clip, binding, toPath);
                }

                ++i;
            }
        }

        void DoManualReplaceFix()
        {
            EditorCurveBinding[] bindings = GetAllCurveBindings(m_Clip);

            string from = m_PathFrom;
            string to = m_PathTo;

            for (int i = 0; i < bindings.Length; ++i)
            {
                EditorCurveBinding binding = bindings[i];

                if (binding.path == from)
                {
                    FixCurveBinding(m_Clip, binding, to);
                }
            }
        }

        string[] LookForExists(GameObject root, string path)
        {
            string[] result = new string[2];

            string[] paths = path.Split('/');

            if (paths.Length == 1)
            {
                result[0] = "";
                result[1] = path;
            }
            else
            {
                string tempPath = "";
                int i = 0;
                for (; i < paths.Length; ++i)
                {
                    tempPath += (i > 0 ? "/" : "") + paths[i];

                    if (root.transform.Find(tempPath) == null)
                    {
                        break;
                    }
                    else
                    {
                        result[0] = tempPath;
                    }
                }

                for (; i < paths.Length; ++i)
                {
                    result[1] += "/" + paths[i];
                }

                //remove first "/"
                result[1] = result[1].Substring(1);
            }

            return result;
        }

        EditorCurveBinding[] GetAllCurveBindings(AnimationClip clip)
        {
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
            EditorCurveBinding[] objectReferenceBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);

            ArrayUtility.AddRange<EditorCurveBinding>(ref bindings, objectReferenceBindings);
            return bindings;
        }

        void FixCurveBinding(AnimationClip clip, EditorCurveBinding binding, string newPath)
        {
            if (binding.isPPtrCurve)
            {
                ObjectReferenceKeyframe[] keyFrames = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                //remove old
                AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
                binding.path = newPath;
                //add new
                AnimationUtility.SetObjectReferenceCurve(clip, binding, keyFrames);
            }
            else
            {
                AnimationCurve aniCurve = AnimationUtility.GetEditorCurve(clip, binding);
                //remove old
                AnimationUtility.SetEditorCurve(clip, binding, null);
                binding.path = newPath;
                //add new
                AnimationUtility.SetEditorCurve(clip, binding, aniCurve);
            }
        }

        Dictionary<string, ArrayList> GetWrongPathBindings()
        {
            Dictionary<string, ArrayList> pathBindingMap = new Dictionary<string, ArrayList>();

            if (m_Clip)
            {
                GameObject root = GetRoot();

                if (root)
                {
                    EditorCurveBinding[] bindings = GetAllCurveBindings(m_Clip);
                    for (int i = 0; i < bindings.Length; ++i)
                    {
                        EditorCurveBinding binding = bindings[i];
                        Debug.Log("binding path=" + binding.path + ",propName=" + binding.propertyName + ",PPtrCurve=" + binding.isPPtrCurve + ",type=" + binding.type);

                        GameObject animatedObject = AnimationUtility.GetAnimatedObject(root, binding) as GameObject;

                        Debug.Log("tt:" + animatedObject + "," + root);

                        if (!animatedObject)
                        {
                            if (!root.transform.Find(binding.path))
                            {
                                //不存在，记录
                                if (!pathBindingMap.ContainsKey(binding.path))
                                {
                                    pathBindingMap[binding.path] = new ArrayList();
                                }

                                pathBindingMap[binding.path].Add(binding);
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("root is null");
                }
            }
            else
            {
                Debug.LogWarning("AnimationClip is null");
            }

            return pathBindingMap;
        }

        GameObject GetRoot()
        {
            return m_Root != null ? m_Root : Selection.activeGameObject;
        }
    }
}