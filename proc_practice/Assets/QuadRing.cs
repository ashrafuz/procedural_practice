using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadRing : MonoBehaviour {

    [Range (0.1f, 2)][SerializeField] float m_RadiusInner;
    [Range (0.1f, 2)][SerializeField] float m_Thickness;
    [Range (3, 32)][SerializeField] int m_AngularSegments;

    float RadiusOuter => m_RadiusInner + m_Thickness;

    private void OnDrawGizmosSelected () {
        MyGizmos.DrawWireCirlce (transform.position, transform.rotation, m_RadiusInner, m_AngularSegments);
        MyGizmos.DrawWireCirlce (transform.position, transform.rotation, RadiusOuter, m_AngularSegments);
    }

    void Start () {

    }

}