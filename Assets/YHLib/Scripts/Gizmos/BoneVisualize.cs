using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH
{
    public class BoneVisualize : MonoBehaviour
    {
        public Transform rootNode;
        public Transform[] childNodes;

        [SerializeField]
        Vector3 m_rootSize = new Vector3(0.02f, 0.02f, .02f);

        [SerializeField]
        Vector2 m_StartSize = new Vector2(0.008f, 0.008f);
        [SerializeField]
        float m_StartOffset = 0.008f;
        [SerializeField]
        Vector2 m_EndSize = new Vector2(0.001f, 0.001f);
        [SerializeField]
        Color m_BoneColor = Color.green;
        [SerializeField]
        Color m_RootBoneColor = Color.blue;
        [SerializeField]
        bool m_DrawWire=false;

        void Start()
        {
            
        }

        void OnDrawGizmos()
        {
            if (rootNode != null)
            {

                if (childNodes == null || childNodes.Length == 0)
                {
                    //get all joints to draw
                    PopulateChildren();
                }


                foreach (Transform child in childNodes)
                {

                    if (child == rootNode)
                    {
                        Gizmos.color = m_RootBoneColor;
                        Gizmos.DrawCube(child.position, m_rootSize);
                    }
                    else
                    {
                        Gizmos.color = m_BoneColor;

                        Mesh mesh = CreateBoneMesh(m_StartSize, m_EndSize, m_StartOffset, Vector3.Distance(child.parent.position, child.position));
                        Quaternion rotation = Quaternion.LookRotation(child.position-child.parent.position);

                        if (m_DrawWire)
                        {
                            Gizmos.DrawWireMesh(mesh, child.parent.position, rotation);
                        }
                        else
                        {
                            Gizmos.DrawMesh(mesh, child.parent.position, rotation);
                        }

                        if (child.childCount == 0)
                        {
                            mesh = CreateBoneMesh(m_StartSize, m_EndSize, m_StartOffset, m_StartOffset * 2);
                            if (m_DrawWire)
                            {
                                Gizmos.DrawWireMesh(mesh, child.position);
                            }
                            else
                            {
                                Gizmos.DrawMesh(mesh, child.position);
                            }
                        }
                    }


                }
            }
        }

        Mesh CreateBoneMesh(Vector2 startSize, Vector2 endSize, float startOffset, float endOffset)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[9];
            //add origin point.
            vertices[0] = Vector3.zero;
            //add start points
            vertices[1] = new Vector3(startSize.x, startSize.y, startOffset); ;
            vertices[2] = new Vector3(-startSize.x, startSize.y, startOffset);
            vertices[3] = new Vector3(-startSize.x, -startSize.y, startOffset);
            vertices[4] = new Vector3(startSize.x, -startSize.y, startOffset);

            //add end points
            vertices[5] = new Vector3(endSize.x, endSize.y, endOffset);
            vertices[6] = new Vector3(-endSize.x, endSize.y, endOffset);
            vertices[7] = new Vector3(-endSize.x, -endSize.y, endOffset);
            vertices[8] = new Vector3(endSize.x, -endSize.y, endOffset);
            int[] triangles = new int[] {
                0,2,1,
                0,3,2,
                0,4,3,
                0,1,4,
                1,6,5,
                1,2,6,
                2,7,6,
                2,3,7,
                3,8,7,
                3,4,8,
                4,5,8,
                4,1,5,
                5,6,8,
                6,7,8
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            //List<Vector3> normals = new List<Vector3>();
            //for (int i = 0; i < mesh.vertices.Length; ++i)
            //{
            //    normals.Add(Vector3.left);
            //}
            //mesh.normals = normals.ToArray();
            return mesh;
        }

        public void PopulateChildren()
        {
            childNodes = rootNode.GetComponentsInChildren<Transform>();
        }
    }
}