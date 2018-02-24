using RosSharp.RosBridgeClient;
using System;
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

    public GameObject controllerHelperLeft;
    public GameObject controllerHelperRight;

    [Tooltip("Parent GameObject which contain robots GameObjects")]
    public GameObject parentGameObject;

    [Tooltip("Multiply radius coverage of selection circle")]
    [Range(1, 50)]
    public int coverageMultiplier = 1;

    public GameObject circleSelector;

    public GameObject weightBar;
    public GameObject weightText;

    private RobotGainArrayPublisher robotGainPublisher;

    //=========//

    private List<GameObject> robots;
    private List<GameObject> selectedRobots;

    private enum LastState { on, off };
    private LastState updateSelection = LastState.off;
    private float radius;

    private Vector3 initalPosition;
    private Material materialSelected;
    private Material materialDefault;

    double weight = 1;

    // Use this for initialization
    void Start () {

        materialDefault = Resources.Load("Materials/Robot", typeof(Material)) as Material;
        materialSelected = Resources.Load("Materials/SelectedRobot", typeof(Material)) as Material;

        robots = new List<GameObject>();
        selectedRobots = new List<GameObject>();
        robotGainPublisher = this.GetComponent<RobotGainArrayPublisher>();

        foreach (Transform child in parentGameObject.transform)
        {
            robots.Add(child.gameObject);
        }
        weightBar.GetComponent<Renderer>().enabled = false;
        weightText.GetComponent<Renderer>().enabled = false;
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

            weightBar.GetComponent<Renderer>().enabled = false;
            weightText.GetComponent<Renderer>().enabled = false;

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

            weightBar.GetComponent<Renderer>().enabled = false;
            weightText.GetComponent<Renderer>().enabled = false;
            controllerHelperLeft.SetActive(false);
            controllerHelperRight.SetActive(false);
            //weightBar.transform.localScale = new Vector3(1, weightBar.transform.localScale.y, weightBar.transform.localScale.z);
            //eightText.GetComponent<TextMesh>().text = weightBar.transform.localScale.x.ToString();

            VoronoiRobotGainArray gainArray = new VoronoiRobotGainArray();
            gainArray.robot_gain_list = new VoronoiRobotGain[selectedRobots.Count];
            gainArray.size = selectedRobots.Count;
            int i = 0;
            foreach (GameObject robot in selectedRobots)
            {
                VoronoiRobotGain gain = new VoronoiRobotGain();
                gain.kp = this.weight;
                //Debug.Log(robot.name);
                gain.id = Int32.Parse(robot.name.Split('_')[1]);
                gainArray.robot_gain_list[i] = gain;
                i++;
            }
            robotGainPublisher.publish(gainArray);
            Debug.Log("published");
        }

        if(controllerLeft.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonOnePress) || controllerLeft.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonTwoPress))
        {
            weightBar.GetComponent<Renderer>().enabled = true;
            weightText.GetComponent<Renderer>().enabled = true;
            controllerHelperLeft.SetActive(true);
            controllerHelperRight.SetActive(true);

            if (controllerLeft.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonOnePress))
            {
                weightBar.transform.localScale -= new Vector3(0.0564f, 0, 0);
                if(weightBar.transform.localScale.x < 1f)
                    weightBar.transform.localScale = new Vector3(1f, weightBar.transform.localScale.y, weightBar.transform.localScale.z);
                weightBar.transform.localPosition = new Vector3(weightBar.transform.localScale.x/2, weightBar.transform.localPosition.y, weightBar.transform.localPosition.z);
                weightText.GetComponent<TextMesh>().text = "New Speed:\n► " + weightBar.transform.localScale.x.ToString("0.00");
            }
            else if (controllerLeft.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonTwoPress))
            {
                weightBar.transform.localScale += new Vector3(0.0564f, 0, 0);
                if (weightBar.transform.localScale.x > 4f)
                    weightBar.transform.localScale = new Vector3(4f, weightBar.transform.localScale.y, weightBar.transform.localScale.z);
                weightBar.transform.localPosition = new Vector3(weightBar.transform.localScale.x / 2, weightBar.transform.localPosition.y, weightBar.transform.localPosition.z);
                weightText.GetComponent<TextMesh>().text = "New Speed:\n► " + weightBar.transform.localScale.x.ToString("0.00");
            }
            else if(controllerLeft.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonOneTouch) ||
                controllerLeft.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.ButtonTwoTouch))
            {

            }
            else
            {
                
            }
            weight = weightBar.transform.localScale.x;
        }
    }
}
