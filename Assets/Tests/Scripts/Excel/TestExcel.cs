using UnityEngine;
using System.Collections;
using System.IO;


public class TestExcel : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        FileStream fs = File.OpenRead(Application.dataPath + "/Tests/Fonts/Txt.fnt");

        Debug.Log(fs.CanRead);
        fs.Close();
        Debug.Log(fs.CanRead);
        fs.Close();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
