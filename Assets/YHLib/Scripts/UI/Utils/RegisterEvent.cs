using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace YH.UI
{
    public class RegisterEvent
    {

        public delegate void ButtonClickHandle(GameObject obj);
        public delegate void ToggleValueChangeHandle(GameObject obj, bool value);

        public static void RegisterButtonsEvent(List<Button> buttons, ButtonClickHandle handle)
        {
            foreach (Button btn in buttons)
            {
                GameObject obj = btn.gameObject;
                btn.onClick.AddListener(delegate ()
                {
                    handle(obj);
                });
            }
        }

        public static void RegisterButtonsEvent(Button[] buttons, ButtonClickHandle handle)
        {
            foreach (Button btn in buttons)
            {
                GameObject obj = btn.gameObject;
                btn.onClick.AddListener(delegate ()
                {
                    handle(obj);
                });
            }
        }

        public static void RegisterTogglesEvent(List<Toggle> toggles, ToggleValueChangeHandle handle)
        {
            foreach (Toggle toggle in toggles)
            {
                GameObject obj = toggle.gameObject;
                toggle.onValueChanged.AddListener(delegate (bool value)
                {
                    handle(obj, value);
                });
            }
        }

        public static void RegisterTogglesEvent(Toggle[] toggles, ToggleValueChangeHandle handle)
        {
            foreach (Toggle toggle in toggles)
            {
                GameObject obj = toggle.gameObject;
                toggle.onValueChanged.AddListener(delegate (bool value)
                {
                    handle(obj, value);
                });
            }
        }
    }
}