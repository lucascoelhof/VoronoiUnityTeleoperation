using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


public class FetchRobots : MonoBehaviour {

    MQTT mMqtt = new MQTT();

    // Use this for initialization
    void Start () {
        mMqtt.Connect();
        mMqtt.Subscribe("pose/robot/+");
    }
	
	// Update is called once per frame
	void Update () {
	}
}
