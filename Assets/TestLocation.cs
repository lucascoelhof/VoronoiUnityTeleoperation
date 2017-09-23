using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.VR;

public class TestLocation : MonoBehaviour {

    public float distanceFromEye = .4f;
    public bool showInFrontOfLeftEye = true;
    Transform smallSphere;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        smallSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        smallSphere.localScale = new Vector3(distanceFromEye, distanceFromEye, distanceFromEye) / 10f;
    }

    void Update()
    {
        
        OVRInput.Update();
        bool aget = OVRInput.Get(OVRInput.Button.One);
        //bool agetdown = OVRInput.GetDown(OVRInput);
        //bool getbutton = OVRInput.GetDown(OVRInput.Button);
        Vector3 head = InputTracking.GetLocalPosition(VRNode.Head);
        Vector3 l_hand = InputTracking.GetLocalPosition(VRNode.LeftHand);
        Vector3 r_hand = InputTracking.GetLocalPosition(VRNode.RightHand);
        Debug.Log("Head> x:" + head.x.ToString("0.00") + " y:" + head.y.ToString("0.00") + " z:" + head.z.ToString("0.00"));
        Debug.Log("Left Hand> x:" + l_hand.x.ToString("0.00") + " y:" + l_hand.y.ToString("0.00") + " z:" + l_hand.z.ToString("0.00"));
        Debug.Log("Right Hand> x:" + r_hand.x.ToString("0.00") + " y:" + r_hand.y.ToString("0.00") + " z:" + r_hand.z.ToString("0.00"));
        //Debug.Log("getdown: " + agetdown.ToString());
        Debug.Log("get: " + aget.ToString());
        //Debug.Log("getbutton: " + getbutton.ToString());
        Vector3 left = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.LeftEye)) * InputTracking.GetLocalPosition(VRNode.LeftEye);
        Vector3 right = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.RightEye)) * InputTracking.GetLocalPosition(VRNode.RightEye);
        Vector3 leftWorld, rightWorld;
        Vector3 offset = (left - right) * 0.5f;
        Matrix4x4 m = cam.cameraToWorldMatrix;
        leftWorld = m.MultiplyPoint(-offset);
        rightWorld = m.MultiplyPoint(offset);
        smallSphere.position = (showInFrontOfLeftEye ? leftWorld : rightWorld) + cam.transform.forward * distanceFromEye;
    }

}
