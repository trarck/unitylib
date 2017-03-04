using UnityEngine;
using System.Collections;

public class RectRenderer : MonoBehaviour {
    [SerializeField]
    float m_Width = 4;

    [SerializeField]
    float m_Height = 4;

    [SerializeField]
    float m_Weight = 0.1f;

    [SerializeField]
    Color m_Color = Color.red;

    Mesh m_Mesh;
    Vector3[] m_Vertices;
    Vector2[] m_UV;
    int[] m_Triangles;
  
	// Use this for initialization
	void Start () {

        CreateVertices();
        CreateUV();

        m_Triangles = new int[]
        {
            1,0,4,
            1,4,5,
            5,6,2,
            2,6,7,
            7,8,9,
            7,9,3,
            9,10,11,
            9,11,4
        };

        m_Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = m_Mesh;

        m_Mesh.vertices = m_Vertices;
        m_Mesh.uv = m_UV;
        m_Mesh.triangles = m_Triangles;
        GetComponent<MeshRenderer>().material.color = m_Color;
        
	}
	
	// Update is called once per frame
	void Update () {
        CreateVertices();
        CreateUV();
        m_Mesh.vertices = m_Vertices;
        m_Mesh.uv = m_UV;
        m_Mesh.triangles = m_Triangles;
        GetComponent<MeshRenderer>().material.color = m_Color;
    }

    /*
       3                       7    2
       9    10               8
        

        4   11               6   5        
        0                           1     
    */
    void CreateVertices()
    {
        m_Vertices = new Vector3[12];
        float halfWidth = m_Width * 0.5f;
        float halfHeight = m_Height * 0.5f;
        //outer pointer
        m_Vertices[0] = new Vector3(-halfWidth,-halfHeight,0);
        m_Vertices[1] = new Vector3(halfWidth, -halfHeight, 0);
        m_Vertices[2] = new Vector3(halfWidth, halfHeight, 0);
        m_Vertices[3] = new Vector3(-halfWidth, halfHeight, 0);

        //inner pointer
        m_Vertices[4] = new Vector3(-halfWidth, -halfHeight+m_Weight, 0);
        m_Vertices[5] = new Vector3(halfWidth, -halfHeight+m_Weight, 0);
        m_Vertices[6] = new Vector3(halfWidth-m_Weight, -halfHeight + m_Weight, 0);

        m_Vertices[7] = new Vector3(halfWidth-m_Weight, halfHeight, 0);
        m_Vertices[8] = new Vector3(halfWidth - m_Weight, halfHeight-m_Weight, 0);
        m_Vertices[9] = new Vector3(-halfWidth, halfHeight-m_Weight, 0);

        m_Vertices[10] = new Vector3(-halfWidth+m_Weight, halfHeight - m_Weight, 0);
        m_Vertices[11] = new Vector3(-halfWidth + m_Weight, -halfHeight+m_Weight, 0);
    }

    void CreateUV()
    {
        float weightWidth = m_Weight / m_Width;
        float weightHeight = m_Weight / m_Height;

        m_UV = new Vector2[12];
        m_UV[0] = new Vector2(0, 0);
        m_UV[1] = new Vector2(1,0);
        m_UV[2] = new Vector2(1,1);
        m_UV[3] = new Vector2(0, 1);
        m_UV[4] = new Vector2(0, weightHeight);
        m_UV[5] = new Vector2(1, weightHeight);
        m_UV[6] = new Vector2(1-weightWidth, weightHeight);
        m_UV[7] = new Vector2(1 - weightWidth, 1);
        m_UV[8] = new Vector2(1 - weightWidth, 1- weightHeight);
        m_UV[9] = new Vector2(0, 1 - weightHeight);
        m_UV[10] = new Vector2(weightWidth, 1 - weightHeight);
        m_UV[11] = new Vector2(weightWidth, weightHeight);
    }

    float width
    {
        get
        {
            return m_Width;
        }
        set
        {
            m_Width = value;
        }
    }

    float height
    {
        get
        {
            return m_Height;
        }
        set
        {
            m_Height = value;
        }
    }

    float weight
    {
        get
        {
            return m_Weight;
        }
        set
        {
            m_Weight = value;
        }
    }

    Color color
    {
        get
        {
            return m_Color;
        }
        set
        {
            m_Color = value;
        }
    }
}
