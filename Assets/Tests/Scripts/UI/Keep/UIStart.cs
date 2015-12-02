using UnityEngine;
using System.Collections.Generic;

using YH;
namespace Keep
{
    public class UIStart : MonoBehaviour
    {
        [SerializeField]
        GameObject[] m_ScenePrefabs;

        // Use this for initialization
        void Start()
        {
            Debug.Log("start");
            foreach(GameObject prefab in m_ScenePrefabs)
            {
                KeepSceneDirector.Instance.scenePrefabMap[prefab.name] = prefab;
            }

            KeepSceneDirector.Instance.RunWithScene("StartScene");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
