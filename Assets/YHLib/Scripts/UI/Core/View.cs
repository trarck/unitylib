using UnityEngine;
using System.Collections;

namespace YH.UI
{
    public class View : MonoBehaviour
    {

        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        public virtual void Hide()
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
