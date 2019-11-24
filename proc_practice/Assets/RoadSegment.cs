using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoadSegment : MonoBehaviour {

    [Range (1, 5)][SerializeField] float m_CurveThickness = 1;
    [Range (0, 1)][SerializeField] float m_CurvePointPercent = 1;
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

        Vector3 percentPoint = GetBezierPoint (m_CurvePointPercent);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere (percentPoint, 0.03f);
        Gizmos.color = Color.white;
    }

    private Vector3 GetBezierPoint (float _t) {
        Vector3 p0 = GetPos (0);
        Vector3 p1 = GetPos (1);
        Vector3 p2 = GetPos (2);
        Vector3 p3 = GetPos (3);

        Vector3 a = Vector3.Lerp (p0, p1, _t);
        Vector3 b = Vector3.Lerp (p1, p2, _t);
        Vector3 c = Vector3.Lerp (p2, p3, _t);

        Vector3 d = Vector3.Lerp (a, b, _t);
        Vector3 e = Vector3.Lerp (b, c, _t);

        return Vector3.Lerp (d, e, _t);
    }
}