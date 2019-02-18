using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace YH
{

    public class FindUtil
    {

        //	private static readonly FindUtil s_Instance =new FindUtil();
        //
        //	static FindUtil()
        //	{
        //
        //	}
        //
        //	public static FindUtil Instance
        //	{
        //		get
        //		{
        //			return s_Instance;
        //		}
        //	}

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
                Queue<Transform> checkList = new Queue<Transform>();
                checkList.Enqueue(parent);

                Transform current, child,fined;
                while (checkList.Count > 0)
                {
                    current = checkList.Dequeue();

                    for (int i = 0, l = current.childCount; i < l; ++i)
                    {
                        child = current.GetChild(i);
                        fined = child.Find(path);
                        if (fined != null)
                        {
                            return fined;
                        }
                        else
                        {
                            checkList.Enqueue(child);
                        }
                    }
                }

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

            Queue<Transform> checkList = new Queue<Transform>();
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
                       
            return list;
        }

        public static List<Transform> SearchTransforms(string path, Transform parent)
        {
            List<Transform> list = new List<Transform>();

            Queue<Transform> checkList = new Queue<Transform>();
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

        public static List<Transform> FindTransforms(string path, Transform parent)
        {

            string[] paths = path.Split('/');

            int l = paths.Length;

            List<Transform> checkList = new List<Transform>();
            checkList.Add(parent);

            string name;
            List<Transform> children = null;

            for (int i = 0; i < l; ++i)
            {
                name = paths[i];

                children = new List<Transform>();

                for (int j = 0, len = checkList.Count; j < len; ++j)
                {
                    children.AddRange(GetChildren(name, checkList[j]));
                }

                checkList = children;
            }

            return checkList;
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

            Transform current,child;
            T temp;
            while (checkList.Count > 0)
            {
                current = checkList.Dequeue();

                for (int i = 0, l = current.childCount; i < l; ++i)
                {
                    child = current.GetChild(i);
                    temp=child.GetComponent<T>();
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
    }

}