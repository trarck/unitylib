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

        public static GameObject FindGameObject(string path, GameObject parent)
        {
            return FindGameObject(path, parent.transform);
        }

        public static GameObject FindGameObject(string path, Transform parent)
        {
            Transform objTransform = parent.Find(path);

            if (objTransform)
            {
                return objTransform.gameObject;
            }
            else
            {
                //在子元素中查找
                for (int i = 0, l = parent.childCount; i < l; ++i)
                {
                    GameObject childFind = FindGameObject(path, parent.GetChild(i));

                    if (childFind)
                    {
                        return childFind;
                    }
                }

                return null;
            }
        }

        public static Transform FindTransform(string path, Transform parent)
        {
            Transform objTransform = parent.Find(path);

            if (objTransform)
            {
                return objTransform;
            }
            else
            {
                //在子元素中查找
                for (int i = 0, l = parent.childCount; i < l; ++i)
                {
                    Transform childFind = FindTransform(path, parent.GetChild(i));

                    if (childFind)
                    {
                        return childFind;
                    }
                }

                return null;
            }
        }

        public static List<GameObject> FindGameObjects(string path, GameObject parent)
        {
            return FindGameObjects(path, parent.transform);
        }

        public static List<GameObject> FindGameObjects(string path, Transform parent)
        {
            List<GameObject> list = new List<GameObject>();

            Transform objTransform = parent.Find(path);

            if (objTransform)
            {
                list.Add(objTransform.gameObject);
            }
            else
            {
                //在子元素中查找
                for (int i = 0, l = parent.childCount; i < l; ++i)
                {
                    List<GameObject> childFinds = FindGameObjects(path, parent.GetChild(i));
                    list.AddRange(childFinds);
                }
            }

            return list;
        }

        public static List<Transform> FindTransforms(string path, Transform parent)
        {
            List<Transform> list = new List<Transform>();

            Transform objTransform = parent.Find(path);

            if (objTransform)
            {
                list.Add(objTransform);
            }
            else
            {
                //在子元素中查找
                for (int i = 0, l = parent.childCount; i < l; ++i)
                {
                    List<Transform> childFinds = FindTransforms(path, parent.GetChild(i));
                    list.AddRange(childFinds);
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

        public static List<Transform> SearchTransforms(string path, Transform parent)
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

        public static List<GameObject> SearchGameObjects(string path, Transform parent)
        {
            List<Transform> transforms = SearchTransforms(path, parent);

            List<GameObject> list = new List<GameObject>(transforms.Count);

            foreach (Transform tf in transforms)
            {
                list.Add(tf.gameObject);
            }
            return list;
        }

        public static List<GameObject> SearchGameObjects(string path, GameObject parent)
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