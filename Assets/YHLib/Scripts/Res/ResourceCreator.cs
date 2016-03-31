using UnityEngine;
using System.Collections.Generic;

namespace YH
{
    public class ResourceCreator : MonoBehaviour
    {
        [System.Serializable]
        struct ResourceItem
        {
            public string key;
            public GameObject value;
        }
        [SerializeField]
        ResourceItem[] m_ResouceItems;
        //资源映色
        Dictionary<string, GameObject> m_ResourcesMap;
        //资源缓存
        Dictionary<string, GameObject> m_ResoucesCache;

        void Awake()
        {
            m_ResoucesCache = new Dictionary<string, GameObject>();
            m_ResourcesMap = new Dictionary<string, GameObject>();
            //build map
            for (int i = 0; i < m_ResouceItems.Length; ++i)
            {
                m_ResourcesMap[m_ResouceItems[i].key] = m_ResouceItems[i].value;
            }
        }

        public GameObject CreateGameObject(string resource, Transform parent, Vector3 position)
        {
            GameObject prefab;
            if (m_ResoucesCache.ContainsKey(resource))
            {
                prefab = m_ResoucesCache[resource];
            }
            else
            {
                prefab = Resources.Load<GameObject>(resource);
                if (prefab != null)
                {
                    m_ResoucesCache[resource] = prefab;
                }
                else
                {
                    Debug.LogError("Can't find resouce for " + resource);
                    return null;
                }
            }

            GameObject obj = Instantiate(prefab);

            Transform objTransform = obj.transform;
            objTransform.SetParent(parent);

            objTransform.localPosition = position;
            objTransform.localEulerAngles = Vector3.zero;
            objTransform.localScale = new Vector3(1, 1, 1);

            return obj;
        }

        public void ClearCache()
        {
            foreach (KeyValuePair<string, GameObject> iter in m_ResoucesCache)
            {
                Resources.UnloadAsset(iter.Value);
            }
            m_ResoucesCache.Clear();
        }

        public Dictionary<string, GameObject> resourcesMap
        {
            set
            {
                m_ResourcesMap = value;
            }

            get
            {
                return m_ResourcesMap;
            }
        }
    }
}