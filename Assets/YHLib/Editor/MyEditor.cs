using UnityEngine;
using System.Collections;
using UnityEditor;

namespace YH
{
    public class MyEditor : Editor
    {


        [MenuItem("MyMenu/My Test")]
        public static void Test()
        {
            Debug.Log("test");


            Debug.Log(Selection.activeObject);
            AnimationClip ac = Selection.activeObject as AnimationClip;

            GameObject panel = GameObject.Find("panel");


            if (ac is AnimationClip)
            {

                Debug.Log("****************---->");
                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(ac);
                for (int i = 0; i < bindings.Length; ++i)
                {
                    EditorCurveBinding binding = bindings[i];
                    Debug.Log("binding path=" + binding.path + ",propName=" + binding.propertyName + ",PPtrCurve=" + binding.isPPtrCurve + ",type=" + binding.type);

                    AnimationCurve aniCurve = AnimationUtility.GetEditorCurve(ac, binding);

                    Object t = AnimationUtility.GetAnimatedObject(panel, binding);

                    if (t)
                    {
                        Debug.Log("test:" + t);
                    }
                    else
                    {
                        Debug.Log("can't find:" + binding.path);
                        string path = binding.path;

                        int pos = path.IndexOf("/");

                        path = path.Substring(pos + 1);

                        Debug.Log("lookFor:" + path);

                        t = panel.transform.Find(path).gameObject;

                        string newPath = AnimationUtility.CalculateTransformPath((t as GameObject).transform, panel.transform);
                        Debug.Log("path:" + newPath);

                        AnimationUtility.SetEditorCurve(ac, binding, null);
                        binding.path = newPath;
                        AnimationUtility.SetEditorCurve(ac, binding, aniCurve);
                    }
                }

                AssetDatabase.SaveAssets();

                Debug.Log("****************<----");
                /*
                Debug.Log("****************---->");
                AnimationClipCurveData[] curves= AnimationUtility.GetAllCurves(ac);

                Debug.Log("l:"+curves.Length);
                for(int i=0;i<curves.Length;++i){
                    Debug.Log(curves[i].path);

                    AnimationMode.StartAnimationMode();
                    curves[i].path="Cuve";
                    AnimationMode.StopAnimationMode();
                }

                Debug.Log("****************<----");
                */
                /*
                AnimationEvent[] events=AnimationUtility.GetAnimationEvents(ac);

                Debug.Log("****************---->");
                Debug.Log(events[0].functionName);
                Debug.Log("****************<----");

                events[0].functionName ="Test";

                AnimationUtility.SetAnimationEvents(ac,events);
                */
            }
            else
            {
                Debug.Log("not AnimationClip");
                Debug.Log((Selection.activeObject));
            }

            //		AnimationUtility.onCurveWasModified = OnCurveWasModified;
            /*
            AnimationClipCurveData[] datas = AnimationUtility.GetAllCurves ();
            for (int i=0; i<datas.Length; ++i) {
                Debug.Log(datas[i].ToString());
            }
            */
        }

        public static void OnCurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType deleted)
        {
            Debug.Log("onEvent:" + binding.path + "," + deleted);
            binding.path = "Cube";
        }
    }
}