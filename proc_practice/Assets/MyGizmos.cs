using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyGizmos {
    public static void DrawWireCirlce (Vector3 _pos, Quaternion _rot, float _radius, int _detail = 32) {
        Vector3[] points = new Vector3[_detail];

        for (int i = 0; i < _detail; i++) {
            float t = i / (float) _detail;
            float angleInRad = t * Maths.TAU;

            Vector2 point2d = Maths.GetUnitVectorByAngle (angleInRad) * _radius;
            points[i] = _pos + _rot * point2d; //vector transformation
        }

        for (int i = 0; i < _detail - 1; i++) {
            Gizmos.DrawLine (points[i], points[i + 1]);
        }
        Gizmos.DrawLine (points[_detail - 1], points[0]);

        //Vector3[] ponts3d = new Vector3[_detail];
    }
}