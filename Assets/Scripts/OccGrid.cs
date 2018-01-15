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
    Vector3 resolutionScale;
    private OccupancyGridManager occupancyGridManager;
    private OccupancyGridPublisher occGridPublisher;
    Vector3 placeToDeploy;

    Boolean lastButtonStatus = false;

	private void Start () {
        occupancyGridManager = this.GetComponent<OccupancyGridManager>();
        occGridPublisher = this.GetComponent<OccupancyGridPublisher>();
        resolution = 0.25f;
        resolutionScale = new Vector3(resolution,1.025f,resolution);
        placeToDeploy = new Vector3(0,0,0);
	}
	
	private void Update () {
        placeToDeploy.x = ((int)Math.Floor(rightHandObject.transform.position.x / resolution))* resolution + resolution / 2; //(int)(rightHandObject.transform.position.x/resolution);
        placeToDeploy.y = 0;
        placeToDeploy.z = ((int)Math.Floor(rightHandObject.transform.position.z / resolution))* resolution + resolution / 2;  //rightHandObject.transform.position.z;//(int)(rightHandObject.transform.position.y/resolution);

        int mapPositionI = (int)Math.Floor(rightHandObject.transform.position.x / resolution)+1;
        int mapPositionJ = (int)Math.Floor(rightHandObject.transform.position.z / resolution);

        ghostCube.transform.localScale = resolutionScale;
        ghostCube.transform.position = placeToDeploy;

        //occupancyGridManager.createCube(rightHandObject.transform.position.x, rightHandObject.transform.position.z);

        //if (controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.TriggerClick))

        bool buttonStatus = controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonOnePress);

        if (buttonStatus && buttonStatus != lastButtonStatus)
        {
            occupancyGridManager.generateNewCube(mapPositionI, mapPositionJ);
            NavigationOccupancyGrid occGrid = occupancyGridManager.getOccupancyGrid();
            occGridPublisher.publish(occGrid);
        }
        lastButtonStatus = buttonStatus;


    }
}
