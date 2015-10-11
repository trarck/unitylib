using UnityEngine;
using System.Collections;
using UnityEditor;

namespace YH
{
    public class BindEvent : Editor
    {


        [MenuItem("MyMenu/Bind Event")]
        public static void Test()
        {

            AnimationUtility.onCurveWasModified = OnCurveWasModified;

        }

        public static void OnCurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType deleted)
        {
            Debug.Log("onEvent:" + binding.path + "," + deleted);

            if (deleted == AnimationUtility.CurveModifiedType.CurveModified)
            {
                binding.path = "Cube";
            }
        }
    }
}