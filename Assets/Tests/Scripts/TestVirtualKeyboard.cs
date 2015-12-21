using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestVirtualKeyboard : MonoBehaviour {
    [SerializeField]
    InputField m_InputField;

    bool m_Focused;

    void Start()
    {
    }

    void Update()
    {
        if(m_InputField)
        {
            if (m_Focused)
            {
                if (!m_InputField.isFocused)
                {
                    m_Focused = false;
                    OnUnFocus();
                }
            }
            else
            {
                if (m_InputField.isFocused)
                {
                    m_Focused = true;
                    OnFocus();
                }
            }
        }
    }

    void OnFocus()
    {
        Debug.Log("Focus");

        Show();
    }

    void OnUnFocus()
    {
        Debug.Log("unFocus");
        Hide();
    }


    public void Show()
    {
        VirtualKeyboard.ShowTouchKeyboard();
    }

    public void Hide()
    {
        VirtualKeyboard.HideTouchKeyboard();
    }

    void OnSelect(BaseEventData data)
    {
        Debug.Log(data);
    }
}
