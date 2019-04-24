using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Newtonsoft.Json;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

namespace YH
{
    public class SceneSaver
    {
        public string dataExt = ".json";

        public string saveDir = null;

        private Scene m_Scene;
        public Scene scene
        {
            get
            {
                return m_Scene;
            }
            set
            {
                m_Scene = value;
            }
        }
        
        public string Save()
        {
            if (!m_Scene.IsValid())
            {
                return null;
            }

            Dictionary<string, List<float>> data = new Dictionary<string, List<float>>();

            Queue<Transform> saveList = new Queue<Transform>();

            foreach(GameObject rootObj in scene.GetRootGameObjects())
            {
                saveList.Enqueue(rootObj.transform);
            }

            while (saveList.Count > 0)
            {
                Transform current = saveList.Dequeue();
                string fullPath = YH.HierarchyUtil.FullPath(current);
                data[fullPath] = GetTransformData(current);

                for(int i = 0, l = current.childCount; i < l; ++i)
                {
                    saveList.Enqueue(current.GetChild(i));
                }
            }

            string sceneDataPath = GetSaveFullPath();
            string content = JsonConvert.SerializeObject(data);
            File.WriteAllText(sceneDataPath, content);
            return sceneDataPath;
        }

        public void Restore(string dataFile)
        {
            if (string.IsNullOrEmpty(dataFile))
            {
                return;
            }

            if (!Path.IsPathRooted(dataFile))
            {
                dataFile = Path.Combine(saveDir, dataFile);
            }

            string content = File.ReadAllText(dataFile);
            Dictionary<string, List<float>> data = JsonConvert.DeserializeObject<Dictionary<string, List<float>>>(content);

            Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

            Queue<Transform> saveList = new Queue<Transform>();

            foreach (GameObject rootObj in scene.GetRootGameObjects())
            {
                saveList.Enqueue(rootObj.transform);
            }

            while (saveList.Count > 0)
            {
                Transform current = saveList.Dequeue();
                string fullPath = YH.HierarchyUtil.FullPath(current);

                if (data.ContainsKey(fullPath))
                {
                    SetTransformData(current, data[fullPath]);
                }

                for (int i = 0, l = current.childCount; i < l; ++i)
                {
                    saveList.Enqueue(current.GetChild(i));
                }
            }
        }

        public void Remove(string dataFile)
        {
            if (!Path.IsPathRooted(dataFile))
            {
                dataFile = Path.Combine(saveDir, dataFile);
            }
            File.Delete(dataFile);
        }

        public List<string> GetSavedFileNames()
        {
            List<string> data = new List<string>();

            if (Directory.Exists(saveDir))
            {
                //get files
                string[] files = Directory.GetFiles(saveDir);

                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);
                    if (fileName.StartsWith(m_Scene.name))
                    {
                        data.Add(Path.GetFileName(filePath));
                    }
                }
            }
            return data;
        }

        List<float> GetTransformData(Transform t)
        {
            List<float> d = new List<float>();
            //local position
            d.Add(t.localPosition.x);
            d.Add(t.localPosition.y);
            d.Add(t.localPosition.z);

            //local rotation
            d.Add(t.localRotation.x);
            d.Add(t.localRotation.y);
            d.Add(t.localRotation.z);
            d.Add(t.localRotation.w);

            //local scale
            d.Add(t.localScale.x);
            d.Add(t.localScale.y);
            d.Add(t.localScale.z);
            return d;
        }

        void SetTransformData(Transform t,List<float> d)
        {
            //local position
            Vector3 localPosition = new Vector3(d[0],d[1],d[2]);
            t.localPosition=localPosition;

            //local rotation
            Quaternion localRotation = new Quaternion(d[3], d[4], d[5], d[6]);
            t.localRotation = localRotation;

            //local scale
            Vector3 localScale = new Vector3(d[7], d[8], d[9]);
            t.localScale = localScale;
        }

        string GetSaveFullPath()
        {
            int maxIndex = 0;

            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            else
            {
                //get files
                string[] files = Directory.GetFiles(saveDir);
                //get last index

                foreach (string fileName in files)
                {
                    string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);

                    if (fileNameNoExt.StartsWith(m_Scene.name))
                    {

                        int p = fileNameNoExt.LastIndexOf("_");
                        int index = int.Parse(fileNameNoExt.Substring(p + 1));
                        if (index > maxIndex)
                        {
                            maxIndex = index;
                        }
                    }
                }

                ++maxIndex;
            }

            string saveDataFile = m_Scene.name + "_" + maxIndex + dataExt;

            return Path.Combine(saveDir, saveDataFile);
        }
    }
}
