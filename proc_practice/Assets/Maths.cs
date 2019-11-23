using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maths {
    public const float TAU = 6.283185f;

    public static Vector2 GetUnitVectorByAngle (float _angRad) {
        return new Vector2 (Mathf.Cos (_angRad), Mathf.Sin (_angRad));
    }

}