using UnityEngine;
using UnityEditor;

namespace YH
{
    [System.Serializable]
    public class FindTab
    {
        [SerializeField]
        string m_SearchPath;

        // Use this for initialization
        public void Init(EditorWindow parent)
        {

        }

        // Update is called once per frame
        public void Update()
        {

        }

        public void OnGUI(Rect pos)
        {
            GUILayout.BeginHorizontal();
            var newPath = EditorGUILayout.TextField("Search Path", m_SearchPath);
            if ((newPath != m_SearchPath) &&
                 (newPath != string.Empty))
            {
                m_SearchPath = newPath;
            }
            GUILayout.EndHorizontal();
        }
    }
}