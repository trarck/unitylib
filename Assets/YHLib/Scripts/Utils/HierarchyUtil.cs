using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace YH
{
    public class HierarchyUtil
    {
        public static GameObject SearchGameObject(string path, GameObject parent)
        {
            return SearchGameObject(path, parent.transform);
        }

        public static GameObject SearchGameObject(string path, Transform parent)
        {
            Transform objTransform = SearchTransform(path, parent);
            if (objTransform)
            {
                return objTransform.gameObject;
            }
            return null;
        }

        public static Transform SearchTransform(string path, Transform parent)
        {
            Transform objTransform = parent.Find(path);

            if (objTransform)
            {
                return objTransform;
            }
            else
            {
                //在子元素中查找
                Queue<Transform> checkList = Pool.QueuePool<Transform>.Get();//new Queue<Transform>();
                checkList.Enqueue(parent);

                Transform current, child, fined;
                while (checkList.Count > 0)
                {
                    current = checkList.Dequeue();

                    for (int i = 0, l = current.childCount; i < l; ++i)
                    {
                        child = current.GetChild(i);
                        fined = child.Find(path);
                        if (fined != null)
                        {
                            Pool.QueuePool<Transform>.Release(checkList);
                            return fined;
                        }
                        else
                        {
                            checkList.Enqueue(child);
                        }
                    }
                }
                Pool.QueuePool<Transform>.Release(checkList);
                return null;
            }
        }

        public static List<GameObject> SearchGameObjects(string path, GameObject parent)
        {
            return SearchGameObjects(path, parent.transform);
        }

        public static List<GameObject> SearchGameObjects(string path, Transform parent)
        {
            List<GameObject> list = new List<GameObject>();

            Queue<Transform> checkList = Pool.QueuePool<Transform>.Get();//new Queue<Transform>();
            checkList.Enqueue(parent);

            Transform current, child, fined;
            while (checkList.Count > 0)
            {
                current = checkList.Dequeue();
                fined = current.Find(path);
                if (fined)
                {
                    list.Add(fined.gameObject);
                }

                for (int i = 0, l = current.childCount; i < l; ++i)
                {
                    child = current.GetChild(i);
                    checkList.Enqueue(child);
                }
            }
            Pool.QueuePool<Transform>.Release(checkList);
            return list;
        }

        public static List<Transform> SearchTransforms(string path, Transform parent)
        {
            List<Transform> list = new List<Transform>();

            Queue<Transform> checkList = Pool.QueuePool<Transform>.Get();//new Queue<Transform>();
            checkList.Enqueue(parent);

            Transform current, child, fined;
            while (checkList.Count > 0)
            {
                current = checkList.Dequeue();
                fined = current.Find(path);
                if (fined)
                {
                    list.Add(fined);
                }

                for (int i = 0, l = current.childCount; i < l; ++i)
                {
                    child = current.GetChild(i);
                    checkList.Enqueue(child);
                }
            }
            Pool.QueuePool<Transform>.Release(checkList);
            return list;
        }

        static List<Transform> GetChildren(string name, Transform parent)
        {
            List<Transform> list = new List<Transform>();

            Transform child;
            for (int i = 0, l = parent.childCount; i < l; ++i)
            {
                child = parent.GetChild(i);
                if (child.name == name)
                {
                    list.Add(child);
                }
            }

            return list;
        }

        static void AppendChildren(Queue queue, string name, Transform parent)
        {
            Transform child;
            for (int i = 0, l = parent.childCount; i < l; ++i)
            {
                child = parent.GetChild(i);
                if (child.name == name)
                {
                    queue.Enqueue(child);
                }
            }

        }

        public static GameObject FindGameObject(string path, GameObject parent)
        {
            return FindGameObject(path, parent.transform);
        }

        public static GameObject FindGameObject(string path, Transform parent)
        {
            Transform objTransform = FindTransform(path, parent);
            if (objTransform)
            {
                return objTransform.gameObject;
            }
            return null;
        }

        public static Transform FindTransform(string path, Transform parent)
        {
            List<Transform> checkList = FindTransforms(path, parent);

            return checkList!=null && checkList.Count > 0 ? checkList[0] : null;
        }

        public static List<Transform> FindTransforms(string path, Transform parent)
        {

            string[] paths = path.Split('/');

            int l = paths.Length;

            Queue<Transform> checkList = Pool.QueuePool<Transform>.Get();

            checkList.Enqueue(parent);

            string name;
            List<Transform> list = new List<Transform>();
            Transform current, child;

            for (int i = 0; i < l; ++i)
            {
                name = paths[i];                
                
                for (int j = 0, len = checkList.Count; j < len; ++j)
                {
                    current = checkList.Dequeue();
                    for (int k = 0, kl = current.childCount; k < kl; ++k)
                    {
                        child = current.GetChild(k);
                        if (child.name == name)
                        {
                            checkList.Enqueue(child);
                        }
                    }
                }

                if (checkList.Count == 0)
                {
                    Pool.QueuePool<Transform>.Release(checkList);
                    return null;
                }
            }

            list.AddRange(checkList);
            Pool.QueuePool<Transform>.Release(checkList);
            return list;
        }

        public static List<GameObject> FindGameObjects(string path, Transform parent)
        {
            List<Transform> transforms = FindTransforms(path, parent);

            List<GameObject> list = new List<GameObject>(transforms.Count);

            foreach (Transform tf in transforms)
            {
                list.Add(tf.gameObject);
            }
            return list;
        }

        public static List<GameObject> FindGameObjects(string path, GameObject parent)
        {
            return SearchGameObjects(path, parent.transform);
        }


        public static List<T> SearchComponents<T>(Transform parent)
        {
            List<T> result = new List<T>();

            Queue<Transform> checkList = new Queue<Transform>();
            checkList.Enqueue(parent);

            Transform current, child;
            T temp;
            while (checkList.Count > 0)
            {
                current = checkList.Dequeue();

                for (int i = 0, l = current.childCount; i < l; ++i)
                {
                    child = current.GetChild(i);
                    temp = child.GetComponent<T>();
                    if (temp != null)
                    {
                        result.Add(temp);
                    }
                    else
                    {
                        checkList.Enqueue(child);
                    }
                }
            }

            return result;
        }

        public static void DestroyChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; --i)
            {
                Object.Destroy(parent.GetChild(i).gameObject);
            }
        }

        public static void DestroyChildrenImmediate(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; --i)
            {
                Object.DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }

        public static string FullPath(Transform transform)
        {
            string path = transform.gameObject.name;

            while (transform.parent)
            {
                transform = transform.parent;
                path = transform.gameObject.name + "/" + path;
            }
            return path;
        }
    }
}
