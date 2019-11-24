using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Mesh2D : ScriptableObject {

    [System.Serializable]
    public struct Vertex {
        public Vector2 points;
        public Vector2 normals;
        [HideInInspector] public float onlyUs; /// uvs, but like v : happy face
    }

    public Vertex[] vertices;
    public int[] lineIndices;

    public int GetVertexCount () {
        return vertices.Length;
    }

    public int GetLineCount () {
        return lineIndices.Length;
    }
}