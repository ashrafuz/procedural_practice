using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public struct OrientedPoint {
    public Quaternion rotation;
    public Vector3 position;

    public Vector3 LocalToWorld (Vector3 _localSpace) {
        return position + rotation * _localSpace;
    }

    public Vector3 LocalToWorldVec (Vector3 _localSpace) {
        return rotation * _localSpace;
    }
}

public class RoadSegment : MonoBehaviour {

    [Range (2, 32)][SerializeField] int m_VerticalSliceCount = 8;
    [Range (1, 5)][SerializeField] float m_CurveThickness = 1;
    [Range (0, 1)][SerializeField] float m_CurvePointPercent = 1;
    [Range (3, 128)][SerializeField] int m_CurveDetailLevel = 3;
    [Range (0.1f, 10)][SerializeField] float m_RoadThickness = 2;

    int VertexCount => m_CurveDetailLevel * 2;

    [SerializeField] MeshFilter m_MeshFilter;
    [SerializeField] Mesh2D m_Shape2D;
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

        Gizmos.color = Color.magenta;

        OrientedPoint percentPoint = GetBezierOrientation (m_CurvePointPercent);
        Gizmos.DrawSphere (percentPoint.position, 0.03f);
        Handles.PositionHandle (percentPoint.position, percentPoint.rotation);

        //void DrawPoint (Vector2 localPos) => Gizmos.DrawSphere (percentPoint.LocalToWorld (localPos), 0.1f);

        Vector3[] verts = m_Shape2D.vertices.Select (v => percentPoint.LocalToWorld (v.points)).ToArray ();

        for (int i = 0; i < m_Shape2D.lineIndices.Length; i += 2) {
            Vector3 a = verts[m_Shape2D.lineIndices[i]];
            Vector3 b = verts[m_Shape2D.lineIndices[i + 1]];

            Gizmos.DrawLine (a, b);
        }

        Gizmos.color = Color.red;
        GenerateRoad ();
    }

    private Mesh mMesh;
    private void GenerateRoad () {
        if (mMesh == null) {
            mMesh = new Mesh ();
            mMesh.name = "RoadSegment";
            m_MeshFilter.sharedMesh = mMesh;
        }

        mMesh.Clear ();

        //vertices
        List<Vector3> allVertsAlongRoad = new List<Vector3> ();
        List<Vector3> allNormals = new List<Vector3> ();

        for (int ring = 0; ring < m_VerticalSliceCount; ring++) {
            float t = ring / (m_VerticalSliceCount - 1f);

            OrientedPoint op = GetBezierOrientation (t);
            for (int i = 0; i < m_Shape2D.vertices.Length; i++) {
                allVertsAlongRoad.Add (op.LocalToWorld (m_Shape2D.vertices[i].points));
                allNormals.Add (op.LocalToWorldVec (m_Shape2D.vertices[i].normals));
                //allNormals.Add (m_Shape2D.vertices[i].normals);//without rotation

            }
        }

        //Trianlges
        List<int> allTriangles = new List<int> ();
        for (int ring = 0; ring < m_VerticalSliceCount - 1; ring++) {
            int rootIndex = ring * m_Shape2D.GetVertexCount ();
            int rootIndexForward = (ring + 1) * m_Shape2D.GetVertexCount ();

            for (int line = 0; line < m_Shape2D.GetLineCount (); line += 2) {
                int lineA = m_Shape2D.lineIndices[line];
                int lineB = m_Shape2D.lineIndices[line + 1];

                int currentA = rootIndex + lineA;
                int currentB = rootIndex + lineB;
                int nextA = rootIndexForward + lineA;
                int nextB = rootIndexForward + lineB;

                allTriangles.Add (currentA);
                allTriangles.Add (nextA);
                allTriangles.Add (nextB);

                allTriangles.Add (nextB);
                allTriangles.Add (currentB);
                allTriangles.Add (currentA);
            }
        }
        mMesh.SetVertices (allVertsAlongRoad);
        mMesh.SetTriangles (allTriangles, 0);
        mMesh.SetNormals (allNormals);

    }

    private void GeneratePitch () {
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

            Vector3 localLeftVector = bezierPoint.LocalToWorld (-Vector3.right * (m_CurveThickness * 0.1f)); //(bezierPoint.position) + (Vector3.left * m_RoadThickness * 0.5f);
            Vector3 localRightVector = bezierPoint.LocalToWorld (Vector3.right * (m_CurveThickness * 0.1f)); //(bezierPoint.position) + (Vector3.right * (m_CurveThickness * 0.1f));

            vertices.Add (localLeftVector);
            normals.Add (-Vector3.up); // in localspace

            vertices.Add (localRightVector);
            normals.Add (-Vector3.up); // in localspace

            Gizmos.DrawSphere (localLeftVector, 0.005f);
            Gizmos.DrawSphere (localRightVector, 0.005f);

            uvs.Add (new Vector2 (1, t));
            uvs.Add (new Vector2 (0, t));
        }

        List<int> triangleIndices = new List<int> ();

        for (int i = 0; i < m_CurveDetailLevel; i++) {
            int rootIndex = i * 2;
            int rtInner = rootIndex + 1;
            int rtOuterNext = (rootIndex + 2);
            int rtInnerNext = (rootIndex + 3);

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