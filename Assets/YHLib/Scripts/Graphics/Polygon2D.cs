using UnityEngine;
using System.Collections.Generic;

/**
 * 在x，y平面内画多边形。
 */
public class Polygon2D : MonoBehaviour
{
    [SerializeField]
    float m_Weight = 0.1f;

    [SerializeField]
    Vector3[] m_Points;
    [SerializeField]
    Vector3[] m_Vertices;
    Vector2[] m_Uvs;
    int[] m_Triangles;

    // Use this for initialization
    void Start()
    {
        if (m_Points.Length > 0)
        {
            BuildPolygon();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    /*
     * 二个向量的角平分线向量
         /
       /
     /
  ---- 角平分线
      \
        \
          \
    边的粗细是由二条边组成的角平分线的长度

     */
    bool CalcAngularBisector(Vector3 a, Vector3 b, out Vector3 bisector)
    {
        //归一化成标准向量
        a.Normalize();
        b.Normalize();

        //二向量共线则不计算平分线
        if (a == b || (a + b == Vector3.zero))
        {
            bisector = Vector3.zero;
            return false;
        }

        bisector = a + b;
        //输出成标准风向量
        bisector.Normalize();
        return true;
    }

    public void BuildPolygon()
    {
        List<Vector3> outPoints = new List<Vector3>();
        List<Vector3> innerPoints = new List<Vector3>();

        Vector3 leftPoint;
        Vector3 rightPoint;

        Vector3 leftEdge;
        Vector3 rightEdge;
        //角平分线向量
        Vector3 bisector;

        float halfWeiget = m_Weight * 0.5f;
        //顺时针取得的平分线方向是里，逆时针是外。即二个向量叉乘z的正负号
        for (int i = 0; i < m_Points.Length; ++i)
        {
            //取前一个点
            leftPoint = m_Points[i == 0 ? m_Points.Length - 1 : i - 1];
            //取后一个点
            rightPoint = m_Points[i == m_Points.Length - 1 ? 0 : i + 1];
            //转化成边向量
            leftEdge = leftPoint - m_Points[i];
            rightEdge = rightPoint - m_Points[i];

            //两边共线，则忽略当前点
            if (CalcAngularBisector(leftEdge, rightEdge, out bisector))
            {
                //计算边的精细，在角平分线和其延长线取相同的长度。
                //判断里外
                if (Vector3.Cross(m_Points[i] - leftPoint, rightEdge).z > 0)
                {
                    outPoints.Add(m_Points[i] - halfWeiget * bisector);
                    innerPoints.Add(m_Points[i] + halfWeiget * bisector);
                }
                else
                {
                    outPoints.Add(m_Points[i] + halfWeiget * bisector);
                    innerPoints.Add(m_Points[i] - halfWeiget * bisector);
                }
            }
        }

        int effectPointCount = outPoints.Count;
        //一个点分为内外二个点
        Debug.Log(effectPointCount);
        //生成mesh的顶点和三角形
        m_Vertices = new Vector3[effectPointCount * 2];
        m_Triangles = new int[effectPointCount * 2 * 3];
        m_Uvs = new Vector2[effectPointCount * 2];

        //连线.组成一条边,由二个三角形组成。内外'外,内内'外'。其中内',外'是下个点的内外点
        int j;
        float u = 0;
        for (int i = 0; i < effectPointCount; ++i)
        {
            u = ((float)i) / (effectPointCount - 1);
            m_Vertices[i * 2] = innerPoints[i];
            m_Vertices[i * 2 + 1] = outPoints[i];
            m_Uvs[i * 2] = new Vector2(u, 0);
            m_Uvs[i * 2 + 1] = new Vector2(u, 1);
            j = i + 1;
            j = j >= effectPointCount ? 0 : j;

            m_Triangles[i * 6] = i * 2;//内
            m_Triangles[i * 6 + 1] = j * 2 + 1;//外'
            m_Triangles[i * 6 + 2] = i * 2 + 1;//外

            m_Triangles[i * 6 + 3] = i * 2;//内
            m_Triangles[i * 6 + 4] = j * 2;//内'
            m_Triangles[i * 6 + 5] = j * 2 + 1;//外'
        }
    }

    public void UpdateMesh(Mesh mesh)
    {
        if (mesh)
        {
            mesh.vertices = m_Vertices;
            mesh.uv = m_Uvs;
            mesh.triangles = m_Triangles;
        }
    }

    public float weight
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

    public Vector3[] points
    {
        get
        {
            return m_Points;
        }

        set
        {
            m_Points = value;
            BuildPolygon();
        }
    }

    public Vector3[] vertices
    {
        get
        {
            return m_Vertices;
        }
    }

    public Vector2[] uvs
    {
        get
        {
            return m_Uvs;
        }
    }

    public int[] triangles
    {
        get
        {
            return m_Triangles;
        }
    }
}
