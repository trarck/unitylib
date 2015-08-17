using UnityEngine;
using System.Collections;
using UnityEditor;

public class AnimationEditor : EditorWindow
{

    string m_Path = "";
    AnimationClip clip;

	[MenuItem ("Animation/Fix Path")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(AnimationEditor));
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        clip = EditorGUILayout.ObjectField("Clip", clip, typeof(AnimationClip), false) as AnimationClip;

        m_Path = EditorGUILayout.TextField("path", m_Path);

        if (GUILayout.Button("fix"))
        {
            AutoFix();
        }
    }

    void AutoFix()
    {
        if (clip)
        {
            GameObject root = Selection.activeGameObject;
            if (root)
            {
                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
                for (int i = 0; i < bindings.Length; ++i)
                {
                    EditorCurveBinding binding = bindings[i];
                    Debug.Log("binding path=" + binding.path + ",propName=" + binding.propertyName + ",PPtrCurve=" + binding.isPPtrCurve + ",type=" + binding.type);

                    

                    GameObject animatedObject = AnimationUtility.GetAnimatedObject(root, binding) as GameObject;

                    if (!animatedObject)
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
                        animatedObject = FindUtil.SearchGameObject(result[1], root);
                        if (!animatedObject)
                        {
                            Debug.LogWarning("can't find:" + result[1]);
                            continue;
                        }

                        string newPath = AnimationUtility.CalculateTransformPath(animatedObject.transform, root .transform);
                        Debug.Log("fix path:" + path+",to:"+newPath);

                        AnimationCurve aniCurve = AnimationUtility.GetEditorCurve(clip, binding);
                        //remove old
                        AnimationUtility.SetEditorCurve(clip, binding, null);
                        binding.path = newPath;
                        //add new
                        AnimationUtility.SetEditorCurve(clip, binding, aniCurve);
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

    string[] LookForExists(GameObject root,string path)
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
                tempPath += (i>0?"/":"") + paths[i];

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
                result[1] += "/"+paths[i];
            }

            //remove first "/"
            result[1] = result[1].Substring(1);
        }

        return result;
    }

	void FixPath(){

	}
}
