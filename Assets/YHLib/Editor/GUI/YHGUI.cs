using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using YH;
namespace YHEditor
{
    public class YHGUI
    {
        public static object DrawElement(object value, Type type, string label = null)
        {
            switch (type.ToString())
            {
                case "System.Int32":
                    value = EditorGUILayout.IntField(label, value != null ? (int)value : 0);
                    break;
                case "System.Int64":
                    value = EditorGUILayout.LongField(label, value != null ? (long)value : 0);
                    break;
                case "System.Single":
                    value = EditorGUILayout.FloatField(label, value != null ? (float)value : 0);
                    break;
                case "System.Double":
                    value = EditorGUILayout.DoubleField(label, value != null ? (double)value : 0);
                    break;

                case "UnityEngine.Vect2":
                    value = EditorGUILayout.Vector2Field(label, value != null ? (Vector2)value : Vector2.zero);
                    break;
                case "UnityEngine.Vect3":
                    value = EditorGUILayout.Vector3Field(label, value != null ? (Vector3)value : Vector3.zero);
                    break;
                case "UnityEngine.Vect4":
                    value = EditorGUILayout.Vector4Field(label, value != null ? (Vector4)value : Vector4.zero);
                    break;
                case "UnityEngine.Rect":
                    value = EditorGUILayout.RectField(label, value != null ? (Rect)value : Rect.zero);
                    break;
                case "UnityEngine.Bounds":
                    value = EditorGUILayout.BoundsField(label, value != null ? (Bounds)value : new Bounds(Vector3.zero, Vector3.zero));
                    break;
                case "UnityEngine.Color":
                    value = EditorGUILayout.ColorField(label, value != null ? (Color)value : Color.black);
                    break;

                case "UnityEngine.AnimationCurve":
                    value = EditorGUILayout.CurveField(label, (AnimationCurve)value);
                    break;

                default:

                    if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                    {
                        value = EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, type, false);
                    }
                    else
                    {
                        value = EditorGUILayout.TextField(label, value != null ? value.ToString() : "");
                    }
                    break;
            }
            return value;
        }

        public static object DrawArray(object value, Type type,ref bool foldout,string label)
        {
            int len = 0;
            if (value == null)
            {
                value = ReflectionUtils.InvokeConstructor(value, new object[] { 0 });
            }
            else
            {
                len = ReflectionUtils.GetLength(value);
            }

            foldout = EditorGUILayout.Foldout(foldout, label);

            if (foldout)
            {
                YHEditorTools.PushLabelWidth(80);

                //数组大小
                int newLen = EditorGUILayout.IntField("size", len);
                if (newLen != len)
                {
                    object newValue = ReflectionUtils.InvokeConstructor(value, new object[] { newLen }); ;

                    for (int i = 0; i < newLen; ++i)
                    {
                        object ele = ReflectionUtils.InvokeMethod(value, "GetValue", new object[] { i < len ? i : len - 1 });
                        ReflectionUtils.InvokeMethod(newValue, "SetValue", new object[] { ele, i });
                    }

                    value = newValue;
                }

                //数组元素
                Type elementType = type.GetElementType();
                for (int i = 0; i < newLen; ++i)
                {
                    object ele = type.GetMethod("GetValue",new Type[] { typeof(int)}).Invoke(value, new object[] { i });
                    object newEle = YHGUI.DrawElement(ele, elementType,"Element "+i);
                    if (ele != newEle)
                    {
                        ReflectionUtils.InvokeMethod(value,"SetValue",new object[] {newEle, i });
                    }
                }

                YHEditorTools.PopLabelWidth();
            }
            return value;
        }
    }
}