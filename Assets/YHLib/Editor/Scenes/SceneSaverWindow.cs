using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Newtonsoft.Json;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using YH;

namespace YHEditor
{
    public class SceneSaverWindow : EditorWindow
    {
        List<string> m_SaveDatas = new List<string>();
        ReorderableList m_List;
        string m_SelectDataFile;
        Vector2 m_ListScrollPosition = Vector2.zero;
        bool m_AutoSave = false;
        float m_Interval = 10*60;

        SceneSaver m_SceneSaver;

        double m_StartTime = 0;

        [MenuItem("Avatar/SceneTransformHelper")]
        public static void ShowMe()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow<SceneSaverWindow>(false, "Scene Saver");
        }

        private void OnEnable()
        {
            m_SceneSaver = new SceneSaver();

            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnSceneOpen;
            EditorApplication.update += OnUpdate;

            Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            m_SceneSaver.saveDir = Path.Combine(Application.temporaryCachePath, scene.name);
            m_SceneSaver.scene = scene;

            m_SaveDatas = m_SceneSaver.GetSavedFileNames();

            m_List = new ReorderableList(m_SaveDatas, typeof(string));

            m_List.displayAdd = false;

            m_List.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Save Datas");
            };

            //m_List.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                
            //};

            m_List.onSelectCallback = (ReorderableList l) => {
                m_SelectDataFile = l.list[l.index] as string;
            };

            m_List.onCanRemoveCallback = (ReorderableList l) => {
                return l.count > 0;
            };

            m_List.onRemoveCallback = (ReorderableList l) => {
                Remove(l.index);
            };

            //m_List.onAddCallback = (ReorderableList l) => {
            //    Debug.Log(l.index);
            //};

            m_StartTime = EditorApplication.timeSinceStartup;

        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= OnSceneOpen;
        }

        private void OnSceneOpen(Scene scene, OpenSceneMode mode)
        {
            m_SceneSaver.saveDir = Path.Combine(Application.temporaryCachePath, scene.name);
            m_SceneSaver.scene = scene;
            m_SaveDatas = m_SceneSaver.GetSavedFileNames();
            m_List.list = m_SaveDatas;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            m_AutoSave = EditorGUILayout.ToggleLeft("AutoSave",m_AutoSave, GUILayout.MinWidth(60));
            YH.YHEditorTools.PushLabelWidth(80);
            m_Interval = EditorGUILayout.FloatField("Interval(s)：", m_Interval, GUILayout.MinWidth(30));
            YH.YHEditorTools.PopLabelWidth();
            EditorGUILayout.EndHorizontal();

            m_ListScrollPosition=EditorGUILayout.BeginScrollView(m_ListScrollPosition);
            m_List.DoLayoutList();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save"))
            {
                Save();
            }

            if (GUILayout.Button("Restore"))
            {
                Restore();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnUpdate()
        {
            if (m_AutoSave && m_Interval > 0)
            {
                if (EditorApplication.timeSinceStartup - m_StartTime > m_Interval)
                {
                    Save();
                    m_StartTime = EditorApplication.timeSinceStartup;
                }
            }
        }

        public void Save()
        {
            string saveFilePath=m_SceneSaver.Save();
            if (!string.IsNullOrEmpty(saveFilePath))
            {
                m_SaveDatas.Add(Path.GetFileName(saveFilePath));
            }
        }

        public void Restore()
        {
            m_SceneSaver.Restore(m_SelectDataFile);
        }

        public void Remove(int i)
        {
            m_SceneSaver.Remove(m_SaveDatas[i]);
            m_SaveDatas.RemoveAt(i);
        }
    }
}

