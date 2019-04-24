using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace YH
{
    public class HierarchyUtil
    {
        class Item
        {
            public Item(Transform t, int d)
            {
                transform = t;
                deep = d;
            }

            public Transform transform;
            public int deep;
        }

        static Transform[] GetChildren(string name, Transform parent)
        {
            List<Transform> list = Pool.ListPool<Transform>.Get();

            Transform child = null;
            for (int i = 0, l = parent.childCount; i < l; ++i)
            {
                child = parent.GetChild(i);
                if (child.name == name)
                {
                    list.Add(child);
                }
            }

            Transform[] result = list.ToArray();
            Pool.ListPool<Transform>.Release(list);
            return result;
        }

        static void GetChildren(string name, Transform parent, List<Transform> list)
        {
            Transform child=null;
            for (int i = 0, l = parent.childCount; i < l; ++i)
            {
                child = parent.GetChild(i);
                if (child.name == name)
                {
                    list.Add(child);
                }
            }
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
        
        public static GameObject SearchGameObject(string name, GameObject parent)
        {
            return SearchGameObject(name, parent.transform);
        }

        public static GameObject SearchGameObject(string name, Transform parent)
        {
            Transform objTransform = SearchTransform(name, parent);
            if (objTransform)
            {
                return objTransform.gameObject;
            }
            return null;
        }

        public static Transform SearchTransform(string name, Transform parent)
        {
            Transform objTransform = parent.Find(name);

            if (objTransform)
            {
                return objTransform;
            }
            else
            {
                //在子元素中查找
                Queue<Transform> checkList = Pool.QueuePool<Transform>.Get();//new Queue<Transform>();
                checkList.Enqueue(parent);

                Transform current, child, finded;
                while (checkList.Count > 0)
                {
                    current = checkList.Dequeue();

                    for (int i = 0, l = current.childCount; i < l; ++i)
                    {
                        child = current.GetChild(i);
                        finded = child.Find(name);
                        if (finded != null)
                        {
                            Pool.QueuePool<Transform>.Release(checkList);
                            return finded;
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

        public static List<GameObject> SearchGameObjects(string name, GameObject parent)
        {
            return SearchGameObjects(name, parent.transform);
        }

        public static List<GameObject> SearchGameObjects(string name, Transform parent)
        {
            List<GameObject> list = new List<GameObject>();

            Queue<Transform> checkList = Pool.QueuePool<Transform>.Get();//new Queue<Transform>();
            checkList.Enqueue(parent);

            Transform current, child, finded;
            while (checkList.Count > 0)
            {
                current = checkList.Dequeue();
                finded = current.Find(name);
                if (finded)
                {
                    list.Add(finded.gameObject);
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

        public static List<Transform> SearchTransforms(string name, Transform parent)
        {
            List<Transform> list = new List<Transform>();

            Queue<Transform> checkList = Pool.QueuePool<Transform>.Get();//new Queue<Transform>();
            checkList.Enqueue(parent);

            Transform current, child, finded;
            while (checkList.Count > 0)
            {
                current = checkList.Dequeue();
                finded = current.Find(name);
                if (finded)
                {
                    list.Add(finded);
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
            return parent.Find(path);

            //string[] paths = path.Split('/');
            //int l = paths.Length;

            //Stack<Item> checkList = Pool.StackPool<Item>.Get();

            //checkList.Push(new Item(parent,0));

            //Item current=null;
            //List<Transform> children = Pool.ListPool<Transform>.Get();
            //Transform result = null;
            //while (checkList.Count > 0)
            //{
            //    current = checkList.Pop();
            //    if (current.deep < l)
            //    {
            //        children.Clear();
            //        GetChildren(paths[current.deep], current.transform, children);
            //        if (children.Count > 0)
            //        {
            //            if (current.deep == l - 1)
            //            {
            //                result = children[0];
            //                break;
            //            }

            //            for (int i = 0; i < children.Count; ++i)
            //            {
            //                checkList.Push(new Item(children[i], current.deep + 1));
            //            }
            //        }
            //    }
            //}

            //Pool.StackPool<Item>.Release(checkList);
            //Pool.ListPool<Transform>.Release(children);
            //return result;
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

        public static Transform FindTransform(string path,UnityEngine.SceneManagement.Scene scene)
        {
            Transform result = null;
            string[] paths = path.Split('/');
            int l = paths.Length;

            Stack<Item> checkList = Pool.StackPool<Item>.Get();
            List<GameObject> rootObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootObjects);
            for (int i = 0;i<rootObjects.Count;  ++i)
            {
                if (rootObjects[i].name == paths[0])
                {
                    if (l == 1)
                    {
                        result = rootObjects[i].transform;
                        break;
                    }
                    checkList.Push(new Item(rootObjects[i].transform, 1));
                }
            }

            if (!result)
            {
                Item current = null;
                List<Transform> children = Pool.ListPool<Transform>.Get();

                while (checkList.Count > 0)
                {
                    current = checkList.Pop();
                    if (current.deep < l)
                    {
                        children.Clear();
                        GetChildren(paths[current.deep], current.transform, children);
                        if (children.Count > 0)
                        {
                            if (current.deep == l - 1)
                            {
                                result = children[0];
                                break;
                            }

                            for (int i = 0; i < children.Count; ++i)
                            {
                                checkList.Push(new Item(children[0], current.deep + 1));
                            }
                        }
                    }
                }
                Pool.ListPool<Transform>.Release(children);
            }

            Pool.StackPool<Item>.Release(checkList);
            return result;
        }

        public static List<Transform> FindTransforms(string path, UnityEngine.SceneManagement.Scene scene)
        {
            string[] paths = path.Split('/');

            int l = paths.Length;

            Queue<Transform> checkList = Pool.QueuePool<Transform>.Get();

            List<GameObject> rootObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootObjects);
            for (int i = 0, k = rootObjects.Count; i < k; ++i)
            {
                checkList.Enqueue(rootObjects[i].transform);
            }

            string name;
            List<Transform> list = new List<Transform>();
            Transform current, child;

            for (int i = 0; i < l; ++i)
            {
                name = paths[i];

                for (int j = 0, len = checkList.Count; j < len; ++j)
                {
                    current = checkList.Dequeue();
                    if (current.name == name)
                    {
                        for (int k = 0, kl = current.childCount; k < kl; ++k)
                        {
                            child = current.GetChild(k);
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

        public static GameObject GetChild(string nameOrPath,GameObject parent)
        {
            if (nameOrPath.IndexOf("/") > -1)
            {
                return FindGameObject(nameOrPath, parent);
            }
            else
            {
                return SearchGameObject(nameOrPath, parent);
            }
        }

        public static GameObject GetChild(string nameOrPath, Transform parent)
        {
            if (nameOrPath.IndexOf("/") > -1)
            {
                return FindGameObject(nameOrPath, parent);
            }
            else
            {
                return SearchGameObject(nameOrPath, parent);
            }
        }

        public static Transform GetChildTransform(string nameOrPath, Transform parent)
        {
            if (nameOrPath.IndexOf("/") > -1)
            {
                return FindTransform(nameOrPath, parent);
            }
            else
            {
                return SearchTransform(nameOrPath, parent);
            }
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

        public static string RelativePath(Transform transform,Transform parent)
        {
            string path = transform.gameObject.name;

            while (transform.parent && transform.parent!=parent)
            {
                transform = transform.parent;
                path = transform.gameObject.name + "/" + path;
            }
            return path;
        }
    }
}
