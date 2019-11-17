using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadGenerator : MonoBehaviour {

    private void Awake () {
        Mesh mesh = new Mesh ();
        mesh.name = "procedural quad";

        List<Vector3> points = new List<Vector3> () {
            new Vector3 (-1, 1),
            new Vector3 (1, 1),
            new Vector3 (-1, -1),
            new Vector3 (1, -1)
        };

        //all normals are pointed towards z axis
        List<Vector3> normals = new List<Vector3> () {
            Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward
        };

        List<Vector2> uvs = new List<Vector2> () {
            new Vector2 (0, 1),
            new Vector2 (1, 1),
            new Vector2 (0, 0),
            new Vector2 (1, 0)
        };

        int[] triangleIndices = new int[] {
            2,
            0,
            1, //first tri
            2,
            1,
            3 // 2nd tri
        };

        mesh.SetVertices (points);

        mesh.SetNormals (normals);
        mesh.SetUVs (0, uvs);
        mesh.triangles = triangleIndices;
        //mesh.RecalculateNormals (); // this process is heavier, so setting normals explicitly might be a good idea
        GetComponent<MeshFilter> ().sharedMesh = mesh;
    }

}