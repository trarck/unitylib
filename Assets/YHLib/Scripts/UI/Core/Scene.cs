using UnityEngine;
using System.Collections;

namespace YH
{
    public class Scene : MonoBehaviour
    {


        public void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        public void Hide()
        {
            OnHide();
            gameObject.SetActive(false);
        }

        public virtual void Reset()
        {

        }

        public virtual void OnShow()
        {

        }

        public virtual void OnHide()
        {

        }
    }
}
