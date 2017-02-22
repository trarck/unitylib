using UnityEngine;
using System.Collections.Generic;

/**
 * 在x，y平面内画多边形。
 */
public class Polygon2DRenderer : MonoBehaviour {

    [SerializeField]
    Color m_Color = Color.red;

    Mesh m_Mesh;
    Polygon2D m_Polygon2D;

    // Use this for initialization
    void Start ()
    {
        m_Mesh = new Mesh();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if(!meshFilter)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = m_Mesh;

        if(GetComponent<MeshRenderer>()==null)
        {
            gameObject.AddComponent<MeshRenderer>();
        }

        m_Polygon2D = GetComponent<Polygon2D>();
        UpdateMesh();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (m_Points.Length > 0)
        //{
        //    BuildPolygon();
        //    UpdateMesh();
        //}
    }
    
    void UpdateMesh()
    {
        m_Polygon2D.BuildPolygon();
        m_Polygon2D.UpdateMesh(m_Mesh);
        GetComponent<MeshRenderer>().material.color = m_Color;
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
