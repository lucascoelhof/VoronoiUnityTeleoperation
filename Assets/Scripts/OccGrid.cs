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

    public GameObject controllerHelper;

    private float resolution;
    Vector3 resolutionScale;
    private OccupancyGridManager occupancyGridManager;
    private OccupancyGridPublisher occGridPublisher;
    Vector3 placeToDeploy;

    private int mapPositionI;
    private int mapPositionJ;

    enum GridModify { Add, Remove };
    private GridModify lastCommand;
    private int lastPositionI = -1;
    private int lastPositionJ = -1;

    private enum UpdateState { Reading, Updated, ToUpdate };
    private UpdateState updateGrid = UpdateState.Updated;

    private void Start () {
        occupancyGridManager = this.GetComponent<OccupancyGridManager>();
        occGridPublisher = this.GetComponent<OccupancyGridPublisher>();
        resolution = 0.25f;
        resolutionScale = new Vector3(resolution,1.025f,resolution);
        placeToDeploy = new Vector3(0,0,0);
	}

    private void Update() {

        ghostCube.GetComponent<Renderer>().enabled = false;

        // Enter 'edit grid' mode if user lean A or B button
        if (controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonOneTouch) ||
            controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonTwoTouch))
        {
            resolution = occupancyGridManager.resolution;
            placeToDeploy.x = ((int)Math.Floor(rightHandObject.transform.position.x / resolution)) * resolution + resolution / 2; //(int)(rightHandObject.transform.position.x/resolution);
            placeToDeploy.y = 0;
            placeToDeploy.z = ((int)Math.Floor(rightHandObject.transform.position.z / resolution)) * resolution + resolution / 2;  //rightHandObject.transform.position.z;//(int)(rightHandObject.transform.position.y/resolution);

            mapPositionI = (int)Math.Floor(rightHandObject.transform.position.z / resolution);
            mapPositionJ = (int)Math.Floor(rightHandObject.transform.position.x / resolution);

            ghostCube.transform.localScale = resolutionScale;
            ghostCube.transform.position = placeToDeploy;
            ghostCube.GetComponent<Renderer>().enabled = true;
            controllerHelper.SetActive(true);
        }
        else
        {
            controllerHelper.SetActive(false);
        }

        // In case the user press both buttons, the function doesn't enter in a loop
        if (controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonOnePress) &&
            controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonTwoPress))
            return;

        if (controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonOnePress) &&
            ( !(lastPositionI == mapPositionI && lastPositionJ == mapPositionJ) ||
              lastCommand == GridModify.Remove) )
        {
            occupancyGridManager.generateNewCube(mapPositionI, mapPositionJ);
            lastPositionI = mapPositionI;
            lastPositionJ = mapPositionJ;
            lastCommand = GridModify.Add;
            updateGrid = UpdateState.ToUpdate;
        }

        if (controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonTwoPress) &&
            ( !(lastPositionI == mapPositionI && lastPositionJ == mapPositionJ) ||
              lastCommand == GridModify.Add) )
        {
            occupancyGridManager.wipeOldCube(mapPositionI, mapPositionJ);
            lastPositionI = mapPositionI;
            lastPositionJ = mapPositionJ;
            lastCommand = GridModify.Remove;
            updateGrid = UpdateState.ToUpdate;
        }

        if (!controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonOnePress) &&
            !controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonTwoPress) &&
            updateGrid == UpdateState.ToUpdate)
        {
            NavigationOccupancyGrid occGrid = occupancyGridManager.getOccupancyGrid();
            occGridPublisher.publish(occGrid);
            updateGrid = UpdateState.Updated;
        }
    }
}
