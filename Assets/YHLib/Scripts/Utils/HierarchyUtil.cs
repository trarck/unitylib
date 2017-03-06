using UnityEngine;

namespace YH
{
    public class HierarchyUtil
    {
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
    }
}
