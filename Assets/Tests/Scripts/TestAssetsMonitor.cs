using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using YH;

public class TestAssetsMonitor : MonoBehaviour {

    float m_Duration = 4.0f;

    float m_Elapsed = 0;
    // Use this for initialization
    void Start () {
        AssetsMonitor am=AssetsMonitor.Instance;
    }
	
	// Update is called once per frame
	void Update () {
        m_Elapsed += Time.fixedDeltaTime;
        if (m_Elapsed >= m_Duration)
        {
            m_Elapsed -= m_Duration;

            if (SceneManager.GetActiveScene().name == "TestAssestUnload1")
            {
                SceneManager.LoadScene("TestAssestUnload2");
            }
            else
            {
                SceneManager.LoadScene("TestAssestUnload1");
            }
        }
    }
}
