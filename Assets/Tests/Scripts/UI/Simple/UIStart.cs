using UnityEngine;
using System.Collections;
using YH;
using YH.UI;

namespace Simple
{
    public class UIStart : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            SimpleSceneDirector.Instance.RunWithScene("StartScene");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}