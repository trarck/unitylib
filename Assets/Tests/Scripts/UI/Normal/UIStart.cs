using UnityEngine;
using System.Collections;
using YH;
using YH.UI;

namespace Normal
{
    public class UIStart : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            Debug.Log("start");
            SceneDirector.Instance.RunWithScene("StartScene");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
