using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public class TestMd5 : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        
    }
	
    string ByteToString(byte[] data)
    {
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; ++i)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
