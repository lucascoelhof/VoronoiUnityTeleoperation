using RosSharp.RosBridgeClient;
using UnityEngine;
using UnityEngine.VR;
using VRTK;

public class GaussianDeformation : MonoBehaviour {

    [Header("VRTK Controller Events")]

    public VRTK_ControllerEvents controllerLeft;
    public VRTK_ControllerEvents controllerRight;

    [Header("Controller Object (used to get Transform)")]

    public GameObject leftHandObject;
    public GameObject rightHandObject;

    [Header("Plane Offset")]

    public GameObject meshOffset;

    [Tooltip("Difference of height between hand and graph")]
    [Range(0f, 1.5f)]
    public float handOffset;


    MeshFilter meshfilter;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Gaussian gaussian;

    float initialHandsDistance;
    bool changingSigma;

    private float sigma;
    
    void Start()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
        meshfilter = GetComponent<MeshFilter>();
        mesh = meshfilter.mesh;
        vertices = mesh.vertices;
        //triangles = mesh.triangles;

        //MeshHelper.Subdivide(mesh, 3);
        meshfilter.mesh = mesh;
        vertices = mesh.vertices;

        gaussian = new Gaussian();

        sigma = 1F;
        gaussian.sigma_x = sigma;
        gaussian.sigma_y = sigma;
        initialHandsDistance = 0;
        changingSigma = false;
    }

    void Update()
    {
        meshfilter = GetComponent<MeshFilter>();
        mesh = meshfilter.mesh;
        vertices = mesh.vertices;

        gameObject.GetComponent<Renderer>().enabled = false;

         if(controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonOnePress)
            || controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.GripClick))
        {
            gameObject.GetComponent<Renderer>().enabled = true;
            if (changingSigma)
            {
                Vector3 distanceHands = rightHandObject.transform.position - leftHandObject.transform.position;
                float finalHandsDistance = Mathf.Sqrt(Mathf.Pow(distanceHands.x, 2) + Mathf.Pow(distanceHands.z, 2)) - initialHandsDistance;
                sigma += finalHandsDistance;
                if (sigma < 0) sigma = 0;
                changingSigma = false;
            }
            if(controllerLeft.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonOnePress)
            || controllerLeft.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.GripClick))
            {
                Vector3 distanceHands = rightHandObject.transform.position - leftHandObject.transform.position;
                initialHandsDistance = Mathf.Sqrt(Mathf.Pow(distanceHands.x, 2) + Mathf.Pow(distanceHands.z, 2));
                changingSigma = true;
            }

            int i = 0;
            while (i < vertices.Length)
            {
                Vector3 relativePosition = new Vector3(0, 0, 0);
                relativePosition.x = (rightHandObject.transform.position.x - meshOffset.transform.position.x) / this.gameObject.transform.localScale.x;
                relativePosition.y = (rightHandObject.transform.position.y - meshOffset.transform.position.y) / this.gameObject.transform.localScale.y - handOffset;
                relativePosition.z = (rightHandObject.transform.position.z - meshOffset.transform.position.z) / this.gameObject.transform.localScale.z;
                vertices[i].y = GaussianPointEvaluate(vertices[i], relativePosition, sigma);
                i++;
            }

            mesh.vertices = vertices;
            meshfilter.mesh = mesh;
        }

    }


    float GaussianPointEvaluate(Vector3 vertice, Vector3 handPose, float sigma=1F)
    {
        OVRInput.Update();
        //handPose = InputTracking.GetLocalPosition(VRNode.RightHand);
        float p_x = Mathf.Pow(vertice.x - handPose.x, 2) / (2 * Mathf.Pow(sigma, 2));
        float p_z = Mathf.Pow(vertice.z - handPose.z, 2) / (2 * Mathf.Pow(sigma, 2));
        float e = Mathf.Exp(-(p_x + p_z));
        float result = handPose.y * e;
        gaussian.a = handPose.y;
        gaussian.x_c = handPose.x;
        gaussian.y_c = handPose.z;
        gaussian.sigma_x = sigma;
        gaussian.sigma_y = sigma;
        return result;
    }

    public Gaussian GetGaussian()
    {
        return gaussian;
    }


}
