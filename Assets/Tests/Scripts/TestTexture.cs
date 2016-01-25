using UnityEngine;
using System.Collections;

public class TestTexture : MonoBehaviour {

    [SerializeField]
    Texture2D m_Texture;

    void ShowBuffer(byte[] buff, int col, int row)
    {
        Debug.Log("show:" + col + "," + row + ":" + buff.Length);
        string lines = "";
        for (int i = row - 1; i >= 0; --i)
        {
            for (int j = 0; j < col; ++j)
            {
                lines += (buff[i * col + j] > 0 ? "1" : "0") + " ";
            }
            lines += "\n";
        }

        Debug.Log(lines);
    }

    byte[] GetBuff(Color[] pixes)
    {
        byte[] buff = new byte[pixes.Length];
        for(int i=0;i<pixes.Length;++i)
        {
            buff[i] = (byte)(255*pixes[i].a);
        }
        return buff;
    }

    byte[] GetBuff(Color32[] pixes)
    {
        byte[] buff = new byte[pixes.Length];
        for (int i = 0; i < pixes.Length; ++i)
        {
            buff[i] =  pixes[i].a;
        }
        return buff;
    }
    // Use this for initialization
    void Start ()
    {
        //Color[] pixes = m_Texture.GetPixels();
        //Debug.Log(pixes.Length);

        byte[] buff = GetBuff(m_Texture.GetPixels32());
        ShowBuffer(buff, m_Texture.width, m_Texture.height);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
