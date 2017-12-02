using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine.VR;
using UnityEngine.UI;


public class OculusTeleoperation : MonoBehaviour {
    
    //MQTT mMqtt = new MQTT();

    //private int frameHash = 0;
    //private int mFrameRefresh = 0;
    
    // Use this for initialization
    void Start () {
        //mMqtt.Connect();
    }
    
    // Update is called once per frame
    void Update () {
        
        
        //if(++frameHash == 10)
        //{
            //OculusPoses.Update();
            //String jsonStr = JsonUtility.ToJson(OculusPoses.poseVR);
            //mMqtt.Publish("lucas_teste_unity_oculus", jsonStr);
            //frameHash = 0;
            //Debug.Log(jsonStr);
        //}
	}

    public void OnGUI() {
         
    }
}
