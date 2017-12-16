using RosSharp;
using RosSharp.RosBridgeClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class OccGrid : MonoBehaviour {

    public GameObject ghostCube;

    [Header("VRTK Controller Events")]

    public VRTK_ControllerEvents controllerLeft;
    public VRTK_ControllerEvents controllerRight;

    [Header("Controller Object (used to get Transform)")]

    public GameObject leftHandObject;
    public GameObject rightHandObject;

    private float resolution;
    private OccupancyGridManager occupancyGridManager;
    Vector3 placeToDeploy;

	private void Start () {
        occupancyGridManager = this.GetComponent<OccupancyGridManager>();
        resolution = 0.5f;
        placeToDeploy = new Vector3(0,0,0);
	}
	
	private void Update () {
        placeToDeploy.x = ((int)Math.Floor(rightHandObject.transform.position.x / resolution))* resolution + resolution / 2; //(int)(rightHandObject.transform.position.x/resolution);
        placeToDeploy.y = 0;
        placeToDeploy.z = ((int)Math.Floor(rightHandObject.transform.position.z / resolution))* resolution + resolution / 2;  //rightHandObject.transform.position.z;//(int)(rightHandObject.transform.position.y/resolution);

        ghostCube.transform.position = placeToDeploy;

        occupancyGridManager.createCube(rightHandObject.transform.position.x, rightHandObject.transform.position.z);

            //if (controllerLeft.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonTwoPress))
            //    occupancyGridManager.createCube(placeToDeploy.x, placeToDeploy.z);
    }
}
