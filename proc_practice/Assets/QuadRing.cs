using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MeshFilter))]
public class QuadRing : MonoBehaviour {
    public enum UvProjection {
        RADIAL,
        TOP_DOWN_PROJECTION
    }

    [Range (0.1f, 2)][SerializeField] float m_RadiusInner;
    [Range (0.1f, 2)][SerializeField] float m_Thickness;
    [Range (3, 32)][SerializeField] int m_AngularSegments;

    [SerializeField] private UvProjection uvProjection = UvProjection.RADIAL;

    float RadiusOuter => m_RadiusInner + m_Thickness;
    int VertexCount => m_AngularSegments * 2;

    private Mesh mMesh;

    private void OnDrawGizmosSelected () {
        MyGizmos.DrawWireCirlce (transform.position, transform.rotation, m_RadiusInner, m_AngularSegments);
        MyGizmos.DrawWireCirlce (transform.position, transform.rotation, RadiusOuter, m_AngularSegments);
    }

    private void Awake () {
        mMesh = new Mesh ();
        mMesh.name = "QuadRing";
        GetComponent<MeshFilter> ().sharedMesh = mMesh;
    }

    private void GenerateMesh () {
        mMesh.Clear (); //clear existing data
        int vCount = VertexCount;
        List<Vector3> vertices = new List<Vector3> ();
        List<Vector3> normals = new List<Vector3> ();
        List<Vector2> uvs = new List<Vector2> ();

        for (int i = 0; i < m_AngularSegments + 1; i++) {
            float t = i / (float) m_AngularSegments;
            float angleInRad = t * Maths.TAU;

            Vector2 dir = Maths.GetUnitVectorByAngle (angleInRad);

            vertices.Add (dir * RadiusOuter);
            normals.Add (Vector3.forward); // in localspace

            vertices.Add (dir * m_RadiusInner);
            normals.Add (Vector3.forward); // in localspace

            // circular projection
            if (uvProjection == UvProjection.RADIAL) {
                uvs.Add (new Vector2 (t, 1));
                uvs.Add (new Vector2 (t, 0));
            } else if (uvProjection == UvProjection.TOP_DOWN_PROJECTION) {
                //have to remap the values from -1,1 to 0,1
                uvs.Add (dir * 0.5f + Vector2.one * 0.5f);
                uvs.Add (dir * (m_RadiusInner / RadiusOuter) * 0.5f + Vector2.one * 0.5f);
            }

        }

        List<int> triangleIndices = new List<int> ();

        for (int i = 0; i < m_AngularSegments; i++) {
            int rootIndex = i * 2;
            int rtInner = rootIndex + 1;
            int rtOuterNext = rootIndex + 2;
            int rtInnerNext = rootIndex + 3;

            triangleIndices.Add (rootIndex);
            triangleIndices.Add (rtOuterNext);
            triangleIndices.Add (rtInnerNext);

            triangleIndices.Add (rootIndex);
            triangleIndices.Add (rtInnerNext);
            triangleIndices.Add (rtInner);
        }

        mMesh.SetVertices (vertices);
        mMesh.SetTriangles (triangleIndices, 0);
        mMesh.SetNormals (normals);
        mMesh.SetUVs (0, uvs);
    }

    private void Update () {
        GenerateMesh ();
    }

}