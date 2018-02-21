using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRobotWeight : MonoBehaviour {

    //private List<GameObject> selectedRobots;
    public RobotGainArrayPublisher gainPublisher;

    // Use this for initialization
    void Start () {
        //selectedRobots = new List<GameObject>();
        if(gainPublisher == null)
        {
            gainPublisher = this.GetComponent<RobotGainArrayPublisher>();
        }

    }

    // Update is called once per frame
    void Update () {
        //selectedRobots = GetComponent<Renderer>();
	}

    void publish(List<VoronoiRobotGain> list_gain)
    {
        //gainPublisher.publish(list_gain);
    }


}
