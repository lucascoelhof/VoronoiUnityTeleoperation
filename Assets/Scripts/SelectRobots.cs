using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SelectRobots : MonoBehaviour {

    public bool changeRobotColorDinamically = false;

    public VRTK_ControllerEvents controllerLeft;
    public VRTK_ControllerEvents controllerRight;

    public GameObject leftHandObject;
    public GameObject rightHandObject;

    [Tooltip("Parent GameObject which contain robots GameObjects")]
    public GameObject parentGameObject;

    [Tooltip("Multiply radius coverage of selection circle")]
    [Range(1, 50)]
    public int coverageMultiplier = 1;

    public GameObject circleSelector;

    //=========//

    private List<GameObject> robots;
    private List<GameObject> selectedRobots;

    private enum LastState { on, off };
    private LastState updateSelection = LastState.off;
    private float radius;

    private Vector3 initalPosition;
    private Material materialSelected;
    private Material materialDefault;

    // Use this for initialization
    void Start () {

        materialDefault = Resources.Load("Materials/Robot", typeof(Material)) as Material;
        materialSelected = Resources.Load("Materials/SelectedRobot", typeof(Material)) as Material;

        robots = new List<GameObject>();
        selectedRobots = new List<GameObject>();

        foreach (Transform child in parentGameObject.transform)
        {
            robots.Add(child.gameObject);
        }

    }

    void selectRobots ()
    {
        foreach (GameObject robot in robots)
        {
            float relativeDistance = Mathf.Sqrt(Mathf.Pow(robot.transform.position.x - initalPosition.x, 2) + Mathf.Pow(robot.transform.position.z - initalPosition.z, 2));

            //Debug.Log("Radius: " + radius + ", Distance: " + relativeDistance);

            GameObject basePart = robot.transform.Find("base_link/body_top/Visuals/unnamed/Cylinder").gameObject;

            if (relativeDistance < radius)
            {
                selectedRobots.Add(robot);
                basePart.GetComponent<Renderer>().material = materialSelected;
            }
            else
            {
                basePart.GetComponent<Renderer>().material = materialDefault;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        circleSelector.GetComponent<Renderer>().enabled = false;

        if (controllerRight.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.TriggerClick))
        {
            circleSelector.GetComponent<Renderer>().enabled = true;

            if (updateSelection == LastState.off)
            {
                initalPosition = rightHandObject.transform.position;
                circleSelector.transform.position = new Vector3 (initalPosition.x, 0.02f, initalPosition.z);
                circleSelector.transform.rotation = Quaternion.identity;

                foreach (GameObject robot in robots)
                {
                    GameObject basePart = robot.transform.Find("base_link/body_top/Visuals/unnamed/Cylinder").gameObject;
                    basePart.GetComponent<Renderer>().material = materialDefault;
                }
            }

            radius = coverageMultiplier * Mathf.Sqrt( Mathf.Pow(rightHandObject.transform.position.x-initalPosition.x,2)+ Mathf.Pow(rightHandObject.transform.position.z - initalPosition.z, 2));
            circleSelector.transform.localScale = new Vector3(2*radius, 0.008f, 2*radius);

            if (changeRobotColorDinamically)
                selectRobots();

            updateSelection = LastState.on;
        }

        else if(updateSelection == LastState.on)
        {
            selectedRobots.Clear();

            //float radius = coverageMultiplier * 2 * Mathf.Sqrt(Mathf.Pow(rightHandObject.transform.position.x - initalPosition.x, 2) + Mathf.Pow(rightHandObject.transform.position.z - initalPosition.z, 2));

            selectRobots();

            updateSelection = LastState.off;
        }
	}
}
