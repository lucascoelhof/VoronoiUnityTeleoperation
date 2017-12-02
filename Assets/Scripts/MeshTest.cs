using UnityEngine;
using UnityEngine.VR;

public class MeshTest : MonoBehaviour {

    MeshFilter meshfilter;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    float initialHandsDistance;
    bool changingSigma;

    private float sigma;

    void Start()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
        meshfilter = GetComponent<MeshFilter>();
        mesh = meshfilter.mesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;

        MeshHelper.Subdivide(mesh, 3);
        meshfilter.mesh = mesh;
        vertices = mesh.vertices;

        sigma = 1F;
        initialHandsDistance = 0;
        changingSigma = false;
    }

    void Update()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        OculusPoses.Update();
        if (OculusPoses.poseVR.Buttons.A.state || OculusPoses.poseVR.Buttons.RHandTrigger > 0.75)
        {
            gameObject.GetComponent<Renderer>().enabled = true;
            if (changingSigma)
            {
                Vector3 distanceHands = OculusPoses.poseVR.RightHand.Position - OculusPoses.poseVR.LeftHand.Position;
                float finalHandsDistance = Mathf.Sqrt(Mathf.Pow(distanceHands.x, 2) + Mathf.Pow(distanceHands.z, 2)) - initialHandsDistance; 
                sigma += finalHandsDistance;
                if (sigma < 0) sigma = 0;
                changingSigma = false;
            }
            if (OculusPoses.poseVR.Buttons.X.state || OculusPoses.poseVR.Buttons.LHandTrigger > 0.75)
            {
                Vector3 distanceHands = OculusPoses.poseVR.RightHand.Position - OculusPoses.poseVR.LeftHand.Position;
                initialHandsDistance = Mathf.Sqrt(Mathf.Pow(distanceHands.x, 2) + Mathf.Pow(distanceHands.z, 2));
                changingSigma = true;
            }

            int i = 0;
            while (i < vertices.Length)
            {
                vertices[i].y = GaussianPointEvaluate(vertices[i], OculusPoses.poseVR.RightHand.Position, sigma);
                i++;
            }

            mesh.vertices = vertices;
            meshfilter.mesh = mesh;
        }
            
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
