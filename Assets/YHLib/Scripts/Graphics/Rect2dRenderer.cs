using UnityEngine;
using System.Collections;

public class Rect2dRenderer : MonoBehaviour {
    [SerializeField]
    Vector3 m_StartPosition;

    [SerializeField]
    Vector3 m_EndPosition;

    [SerializeField]
    Color m_Color = Color.red;

    Mesh m_Mesh;
    Polygon2D m_Polygon2D;

    // Use this for initialization
    void Start () {
        m_Mesh = new Mesh();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = m_Mesh;

        if (GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
        }

        m_Polygon2D = GetComponent<Polygon2D>();

        //UpdateMesh();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void UpdateMesh()
    {
        Vector3[] points = new Vector3[4];
        points[0] = m_StartPosition;
        points[2] = m_EndPosition;
        points[1] = new Vector3(m_EndPosition.x, m_StartPosition.y, m_EndPosition.z);
        points[3] = new Vector3(m_StartPosition.x, m_EndPosition.y, m_StartPosition.z);
        m_Polygon2D.points = points;
        m_Polygon2D.BuildPolygon();
        m_Polygon2D.UpdateMesh(m_Mesh);
        GetComponent<MeshRenderer>().material.color = m_Color;
    }

    public Color color
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

    public Vector3 startPosition
    {
        get
        {
            return m_StartPosition;
        }
        set
        {
            m_StartPosition = value;
        }
    }

    public Vector3 endPosition
    {
        get
        {
            return m_EndPosition;
        }

        set
        {
            m_EndPosition = value;
            UpdateMesh();
        }
    }
}
