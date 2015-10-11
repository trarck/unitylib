using UnityEngine;
using System.Collections;

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

        public static ArrayList FindGameObjects(string path, GameObject parent)
        {
            return FindGameObjects(path, parent.transform);
        }

        public static ArrayList FindGameObjects(string path, Transform parent)
        {
            ArrayList list = new ArrayList();

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
                    ArrayList childFinds = FindGameObjects(path, parent.GetChild(i));
                    list.AddRange(childFinds);
                }
            }

            return list;
        }

        public static ArrayList FindTransforms(string path, Transform parent)
        {
            ArrayList list = new ArrayList();

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
                    ArrayList childFinds = FindTransforms(path, parent.GetChild(i));
                    list.AddRange(childFinds);
                }
            }

            return list;
        }

        static ArrayList GetChildren(string name, Transform parent)
        {
            ArrayList list = new ArrayList();

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

        public static ArrayList SearchTransforms(string path, Transform parent)
        {

            string[] paths = path.Split('/');

            int l = paths.Length;

            ArrayList checkList = new ArrayList();
            checkList.Add(parent);

            string name;
            ArrayList children = null;

            for (int i = 0; i < l; ++i)
            {
                name = paths[i];

                children = new ArrayList();

                for (int j = 0, len = checkList.Count; j < len; ++j)
                {
                    children.AddRange(GetChildren(name, checkList[j] as Transform));
                }

                checkList = children;
            }

            return checkList;
        }

        public static ArrayList SearchGameObjects(string path, Transform parent)
        {
            ArrayList transforms = SearchTransforms(path, parent);

            ArrayList list = new ArrayList(transforms.Count);

            foreach (Transform tf in transforms)
            {
                list.Add(tf.gameObject);
            }
            return list;
        }

        public static ArrayList SearchGameObjects(string path, GameObject parent)
        {
            return SearchGameObjects(path, parent.transform);
        }
    }

}