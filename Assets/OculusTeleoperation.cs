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

    private MqttClient mMqttClient;

    private const string HOSTNAME = "150.164.212.253";//"150.164.212.253";

    int frameHash = 0;

    private int mFrameRefresh = 0;

    public static bool[] streamingVideo = {false,false};
    public static bool[] renderingTexture = {false,false};
    public static Texture2D textureLeft;

    void MqttConnect(string hostname) {
        mMqttClient = new MqttClient(hostname);
        mMqttClient.MqttMsgPublishReceived += onMqttMessage;
        string clientId = System.Guid.NewGuid().ToString();
        mMqttClient.Connect(clientId);
    }

    void MqttPublish(string topic, string message, int qos) {
        byte qos_type;

      
        if (qos == 1)
            qos_type = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
        else if (qos == 2)
            qos_type = MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE;
        else
            qos_type = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE;
            
        mMqttClient.Publish(topic, Encoding.UTF8.GetBytes(message), qos_type, false);
    }
    
    void onMqttMessage(object sender, MqttMsgPublishEventArgs e) {
        string msg = System.Text.Encoding.UTF8.GetString(e.Message);
        Debug.Log("Received message from Broker: " + msg);
    }

    void pose_publish ()
    {
        float ts = 0;

        while (true){
            // ts = track 
        }
    }


    // Use this for initialization
    void Start () {
        textureLeft = new Texture2D(2, 2);
        MqttConnect(HOSTNAME);
    }
    
    // Update is called once per frame
    void Update () {
        if(++frameHash == 2)
        {
            OculusPoses.Update();
            String jsonStr = JsonUtility.ToJson(OculusPoses.poseVR);
            MqttPublish("lucas_teste_unity_oculus", jsonStr, 0);
            frameHash = 0;
            //Debug.Log(jsonStr);
        }
	}
    public void OnGUI() {
         
    }
}
