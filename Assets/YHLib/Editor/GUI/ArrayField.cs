using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using YH;

namespace YHEditor
{
    public class ArrayField
    {

        public string label;
        public bool foldout;

        public void DrawArray(object value, Type type)
        {
            int len = 0;
            if (value == null)
            {
                ConstructorInfo[] cs = type.GetConstructors();
                object[] ps = new object[] { 0 };
                value = cs[0].Invoke(ps);
            }
            else
            {
                MethodInfo getLength = type.GetMethod("get_Length");
                len = ReflectionUtils.GetLength(value);
            }

            foldout = EditorGUILayout.Foldout(foldout, label);

            if (foldout)
            {
                //draw elements
                int size = EditorGUILayout.IntField("size", len);
                if (size != len)
                {

                }

                Type elementType = type.GetElementType();

                for (int i=0;i< size; ++i)
                {
                    object ele = type.GetMethod("GetValue").Invoke(value, new object[] { i });
                    object newEle = YHGUI.DrawElement(ele, elementType);
                    if (ele != newEle)
                    {
                        type.GetMethod("SetValue").Invoke(value, new object[] { i, newEle });
                    }
                }
            }
        }

    }
}