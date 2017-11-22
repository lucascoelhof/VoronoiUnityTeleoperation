using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VR;

public class MeshTest : MonoBehaviour {

    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    int[] triangles;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        normals = mesh.normals;
        triangles = mesh.triangles;

        int v = mesh.vertices.Length;

        List<Vector3> n_list = mesh.vertices.ToList();

        for(int i=0; i<v-1; i++)
        {
            Vector3 n1 = mesh.vertices[i];
            Vector3 n2 = mesh.vertices[i+1];
            Vector3 n = new Vector3((n1.x + n2.x) / 2, (n1.y + n2.y) / 2, (n1.z + n2.z) / 2);
            n_list.Add(n);
        }
        mesh.vertices = n_list.ToArray();

    }

    void Update()
    {
        int i = 0;
        while (i < vertices.Length)
        {
            vertices[i].y = GaussianPointEvaluate(vertices[i], OculusPoses.poseVR.RightHand.Position);
            i++;
        }
        mesh.vertices = vertices;
    }


    float GaussianPointEvaluate(Vector3 vertice, Vector3 handPose, float sigma=1F)
    {
        OVRInput.Update();
        handPose = InputTracking.GetLocalPosition(VRNode.RightHand);
        float p_x = Mathf.Pow(vertice.x - handPose.x, 2) / (2 * Mathf.Pow(sigma, 2));
        float p_z = Mathf.Pow(vertice.z - handPose.z, 2) / (2 * Mathf.Pow(sigma, 2));
        float e = Mathf.Exp(-(p_x + p_z));
        float result = handPose.y * e;
        return result;
    }


}
