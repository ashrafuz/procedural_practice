using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadRing : MonoBehaviour {

    [Range (0.1f, 2)][SerializeField] float m_RadiusInner;
    [Range (0.1f, 2)][SerializeField] float m_Thickness;
    [Range (0.1f, 2)][SerializeField] float m_AngularSegments;

    void Start () {

    }

}