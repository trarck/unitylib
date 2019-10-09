using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace YH.Update
{
    public class GenerateWindow : EditorWindow
    {
        //版本目录
        string m_Path = "";

        //是否保留中间目录
        bool m_keepPatchDirs = false;

        //是否使用二进制差分
        bool m_useBinaryDiff = false;

        //使用差分的文件最小长度
        int m_UseBinaryDiffFileSize=128;

        //不使用差分的文件目录
        string m_NoBinaryDiffDirs;

        //不使用差分的文件扩展名
        string m_NoBinaryDiffExts;
       

        [MenuItem("Assets/Generate Patch")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(GenerateWindow));
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("version patch generate", EditorStyles.boldLabel);

            //选择目录
            EditorGUILayout.BeginHorizontal();
            m_Path = EditorGUILayout.TextField("版本资源目录:", m_Path);

            if (GUILayout.Button("Select path", GUILayout.Width(100)))
            {
                m_Path = EditorUtility.SaveFolderPanel("Set version Directory", m_Path, "");
            }
            EditorGUILayout.EndHorizontal();

            m_keepPatchDirs = EditorGUILayout.Toggle("保留补丁目录:", m_keepPatchDirs);

            m_useBinaryDiff = EditorGUILayout.Toggle("使用二进制差分:",m_useBinaryDiff);

            if (m_useBinaryDiff)
            {
                //黑名单目录
                m_UseBinaryDiffFileSize = EditorGUILayout.IntField("使用差分的最小文件长度:", m_UseBinaryDiffFileSize);

                //黑名单目录
                m_NoBinaryDiffDirs = EditorGUILayout.TextField("不使用差分目录(;分割):", m_NoBinaryDiffDirs);

                //黑名扩展名
                m_NoBinaryDiffExts = EditorGUILayout.TextField("不使用差分扩展名(;分割):", m_NoBinaryDiffExts);
            }
            if (GUILayout.Button("generate"))
            {
                Generate(m_Path);
            }
        }

        void Generate(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Select version patch dir", "the path is empty","ok");
                return;
            }

            GeneratePatch gen = new GeneratePatch();
            gen.UseDiffPatch = m_useBinaryDiff;
            gen.RemovePatchsAfterPack = !m_keepPatchDirs;
            if (m_useBinaryDiff)
            {
                if (m_UseBinaryDiffFileSize >= 0)
                {
                    gen.PatchMinFileSize = m_UseBinaryDiffFileSize;
                }

                if (!string.IsNullOrEmpty(m_NoBinaryDiffDirs))
                {
                    gen.PatchBlackDirs = new List<string>(m_NoBinaryDiffDirs.Split(';'));
                }

                if (!string.IsNullOrEmpty(m_NoBinaryDiffExts))
                {
                    gen.PatchBlackFileExts = new List<string>(m_NoBinaryDiffExts.Split(';'));
                }
            }

            gen.Generate(m_Path);
        }
    }
}