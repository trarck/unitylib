using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fps : MonoBehaviour
{
    int m_Frame = 0;
    float m_Elapsed = 0;
    string m_CurrentFrame = "";

    private void Update()
    {
        ++m_Frame;
        m_Elapsed += Time.deltaTime;
        if (m_Elapsed > 1)
        {
            m_CurrentFrame = ((int)(m_Frame / m_Elapsed)).ToString();
            m_Elapsed = 0;
            m_Frame=0;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 200, 30, 60));
        GUILayout.Label(m_CurrentFrame);
        GUILayout.EndArea();
    }
}
