using RosSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchHands : PoseTransformManager {
    public enum Hand
    {
        left,
        right
    }
    public Hand hand;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        OculusPoses.Update();
        this.transform.position = hand == Hand.left ? OculusPoses.poseVR.LeftHand.Position : OculusPoses.poseVR.RightHand.Position;
        this.transform.rotation = hand == Hand.left ? OculusPoses.poseVR.LeftHand.Orientation : OculusPoses.poseVR.RightHand.Orientation;
	}
}
