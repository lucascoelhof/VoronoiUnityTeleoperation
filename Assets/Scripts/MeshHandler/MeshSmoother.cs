using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSmoother : MonoBehaviour {

    MeshFilter meshfilter;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    [Header("Subdive Mesh")]

    [Tooltip("Divide meshes in submeshes to generate more triangles and make the mesh smoother if deformed")]
    [Range(0, 10)]
    public int timesToSubdivide;

    // Use this for initialization
    void Start () {
        gameObject.GetComponent<Renderer>().enabled = true;
        meshfilter = GetComponent<MeshFilter>();
        mesh = meshfilter.mesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;

        MeshHelper.Subdivide(mesh, timesToSubdivide);
        meshfilter.mesh = mesh;
        vertices = mesh.vertices;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
