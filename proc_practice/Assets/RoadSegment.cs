using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public struct OrientedPoint {
    public Quaternion rotation;
    public Vector3 position;
}

public class RoadSegment : MonoBehaviour {

    [Range (1, 5)][SerializeField] float m_CurveThickness = 1;
    [Range (0, 1)][SerializeField] float m_CurvePointPercent = 1;
    [Range (3, 128)][SerializeField] int m_CurveDetailLevel = 3;
    [Range (0.1f, 10)][SerializeField] float m_RoadThickness = 2;

    int VertexCount => m_CurveDetailLevel * 2;

    [SerializeField] MeshFilter m_MeshFilter;
    [SerializeField] Transform[] points = new Transform[4];

    Vector3 GetPos (int i) => points[i].position;

    private void OnDrawGizmos () {

        for (int i = 0; i < 4; i++) {
            Gizmos.DrawSphere (GetPos (i), 0.02f);
        }

        Handles.DrawBezier (
            GetPos (0), // start pos
            GetPos (3), // end pos
            GetPos (1), // start tangent
            GetPos (2), // end tangent
            Color.white, EditorGUIUtility.whiteTexture, m_CurveThickness);

        Gizmos.color = Color.green;

        OrientedPoint percentPoint = GetBezierOrientation (m_CurvePointPercent);
        Gizmos.DrawSphere (percentPoint.position, 0.03f);
        Handles.PositionHandle (percentPoint.position, percentPoint.rotation);

        Gizmos.color = Color.red;

        GenerateRoad ();
    }

    private Mesh mMesh;
    private void GenerateRoad () {
        if (mMesh == null) {
            mMesh = new Mesh ();
            mMesh.name = "Road Segment";
            m_MeshFilter.sharedMesh = mMesh;
        }

        mMesh.Clear (); //clear existing data
        int vCount = VertexCount;
        List<Vector3> vertices = new List<Vector3> ();
        List<Vector3> normals = new List<Vector3> ();
        List<Vector2> uvs = new List<Vector2> ();

        for (int i = 0; i < m_CurveDetailLevel + 1; i++) {
            float t = i / (float) m_CurveDetailLevel;

            OrientedPoint bezierPoint = GetBezierOrientation (t);

            Vector3 localLeftVector = (bezierPoint.position) + (Vector3.left * m_RoadThickness * 0.5f);
            Vector3 localRightVector = (bezierPoint.position) + (Vector3.right * m_RoadThickness * 0.5f);

            vertices.Add (localLeftVector);
            normals.Add (-Vector3.up); // in localspace

            vertices.Add (localRightVector);
            normals.Add (-Vector3.up); // in localspace

            Gizmos.DrawSphere (localLeftVector, 0.01f);
            Gizmos.DrawSphere (localRightVector, 0.01f);

            uvs.Add (new Vector2 (1, t));
            uvs.Add (new Vector2 (0, t));
        }

        List<int> triangleIndices = new List<int> ();

        for (int i = 0; i < m_CurveDetailLevel; i++) {
            int rootIndex = i * 2;
            int rtInner = rootIndex + 1;
            int rtOuterNext = (rootIndex + 2);
            int rtInnerNext = (rootIndex + 3);

            triangleIndices.Add (rtOuterNext);
            triangleIndices.Add (rootIndex);
            triangleIndices.Add (rtInnerNext);

            triangleIndices.Add (rtInnerNext);
            triangleIndices.Add (rootIndex);
            triangleIndices.Add (rtInner);
        }

        mMesh.SetVertices (vertices);
        mMesh.SetTriangles (triangleIndices, 0);
        mMesh.SetNormals (normals);
        mMesh.SetUVs (0, uvs);

    }

    private OrientedPoint GetBezierOrientation (float _t) {
        Vector3 p0 = GetPos (0);
        Vector3 p1 = GetPos (1);
        Vector3 p2 = GetPos (2);
        Vector3 p3 = GetPos (3);

        Vector3 a = Vector3.Lerp (p0, p1, _t);
        Vector3 b = Vector3.Lerp (p1, p2, _t);
        Vector3 c = Vector3.Lerp (p2, p3, _t);

        Vector3 d = Vector3.Lerp (a, b, _t);
        Vector3 e = Vector3.Lerp (b, c, _t);

        OrientedPoint op;
        op.position = Vector3.Lerp (d, e, _t);
        op.rotation = Quaternion.LookRotation ((e - d).normalized);

        return op;
    }
}