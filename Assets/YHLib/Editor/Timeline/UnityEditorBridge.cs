using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Globalization;

namespace YHEditor.Timeline
{
    public class UnityEditorBridge
    {
        

        public static void ApplyWireMaterial()
        {
            Type t = typeof(HandleUtility);
            MethodInfo m= t.GetMethod("ApplyWireMaterial");
            if (m!=null)
            {
                m.Invoke(null,null);
            }
        }

        private static readonly GUIContent s_Text = new GUIContent();
        public static GUIContent TempContent(string t)
        {
            s_Text.image = null;
            s_Text.text = t;
            s_Text.tooltip = null;
            return s_Text;
        }

        public static string Format(string fmt, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture.NumberFormat, fmt, args);
        }

        public static int GetPermanentControlID()
        {
            Type t = typeof(GUIUtility);
            MethodInfo m = t.GetMethod("GetPermanentControlID");
            if (m != null)
            {
               return (int) m.Invoke(null, null);
            }
            return -1;
        }

        #region Field

        static MethodInfo s_DoFloatFieldMI=null;
        static object[] s_DoFloatFieldArgs = null;

        public static float FloatField(Rect position, int id, float value)
        {
            if (s_DoFloatFieldMI == null)
            {
                Type editorGUIType = typeof(EditorGUI);
                MemberInfo[] members = editorGUIType.GetMember("DoFloatField");
                foreach (var m in members)
                {
                    MethodInfo mi = m as MethodInfo;
                    if (mi != null)
                    {
                        ParameterInfo[] parameters = mi.GetParameters();
                        if (parameters != null && parameters.Length == 8)
                        {
                            s_DoFloatFieldMI = mi;
                            break;
                        }
                    }
                }

                if (s_DoFloatFieldMI == null)
                {
                    return 0;
                }

                if (s_DoFloatFieldArgs == null)
                {
                    s_DoFloatFieldArgs = new object[8];
                    FieldInfo fieldInfo= editorGUIType.GetField("s_RecycledEditor");
                    s_DoFloatFieldArgs[0]=fieldInfo.GetValue(null);
                    s_DoFloatFieldArgs[2] = new Rect(0, 0, 0, 0);
                    s_DoFloatFieldArgs[5] = "g7";
                    s_DoFloatFieldArgs[6] = EditorStyles.numberField;
                    s_DoFloatFieldArgs[7] = false;
                }
            }
            s_DoFloatFieldArgs[1] = position;
            s_DoFloatFieldArgs[3] = id;
            s_DoFloatFieldArgs[4] = value;
            return (float)s_DoFloatFieldMI.Invoke(null, s_DoFloatFieldArgs);
        }

        static MethodInfo s_DoIntFieldMI = null;
        static object[] s_DoIntFieldArgs = null;

        public static int IntField(Rect position, int id, int value)
        {
            if (s_DoIntFieldMI == null)
            {
                Type editorGUIType = typeof(EditorGUI);
                MemberInfo[] members = editorGUIType.GetMember("DoIntField");
                foreach (var m in members)
                {
                    MethodInfo mi = m as MethodInfo;
                    if (mi != null)
                    {
                        ParameterInfo[] parameters = mi.GetParameters();
                        if (parameters != null && parameters.Length == 9)
                        {
                            s_DoIntFieldMI = mi;
                            break;
                        }
                    }
                }

                if (s_DoIntFieldMI == null)
                {
                    return 0;
                }

                if (s_DoIntFieldArgs == null)
                {
                    s_DoIntFieldArgs = new object[9];
                    FieldInfo fieldInfo = editorGUIType.GetField("s_RecycledEditor");
                    s_DoIntFieldArgs[0] = fieldInfo.GetValue(null);
                    s_DoIntFieldArgs[2] = new Rect(0, 0, 0, 0);
                    s_DoIntFieldArgs[5] = "#######0";
                    s_DoIntFieldArgs[6] = EditorStyles.numberField;
                    s_DoIntFieldArgs[7] = false;
                    s_DoIntFieldArgs[8] = 0f;
                }
            }
            s_DoIntFieldArgs[1] = position;
            s_DoIntFieldArgs[3] = id;
            s_DoIntFieldArgs[4] = value;
            return (int)s_DoIntFieldMI.Invoke(null, s_DoIntFieldArgs);
        }

        static MethodInfo s_DoTextFieldMI = null;
        static object[] s_DoTextFieldArgs = null;

        public static string TextField(Rect position, int id, string text, string allowedCharacters,out bool changed)
        {
            if (s_DoTextFieldMI == null)
            {
                Type editorGUIType = typeof(EditorGUI);
                MemberInfo[] members = editorGUIType.GetMember("DoIntField");
                foreach (var m in members)
                {
                    MethodInfo mi = m as MethodInfo;
                    if (mi != null)
                    {
                        ParameterInfo[] parameters = mi.GetParameters();
                        if (parameters != null && parameters.Length ==10)
                        {
                            s_DoTextFieldMI = mi;
                            break;
                        }
                    }
                }

                if (s_DoTextFieldMI == null)
                {
                    changed = false;
                    return null;
                }

                if (s_DoTextFieldArgs == null)
                {
                    s_DoTextFieldArgs = new object[10];
                    FieldInfo fieldInfo = editorGUIType.GetField("s_RecycledEditor");
                    s_DoTextFieldArgs[0] = fieldInfo.GetValue(null);
                    s_DoTextFieldArgs[4] = EditorStyles.numberField;
                    s_DoTextFieldArgs[7] = false;
                    s_DoTextFieldArgs[8] = false;
                    s_DoTextFieldArgs[9] = false;
                }
            }

            changed = false;
            s_DoTextFieldArgs[1] = id;
            s_DoTextFieldArgs[2] = position;
            s_DoTextFieldArgs[3] = text;
            s_DoTextFieldArgs[5] = allowedCharacters;
            s_DoTextFieldArgs[6] = changed;
            string ret= (string)s_DoTextFieldMI.Invoke(null, s_DoTextFieldArgs);
            changed = (bool)s_DoTextFieldArgs[6];
            return ret;
        }

        #endregion
    }

}
