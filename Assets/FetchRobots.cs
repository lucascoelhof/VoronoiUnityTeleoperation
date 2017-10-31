using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


public class FetchRobots : MonoBehaviour {

    MQTT mMqtt = new MQTT();
    Dictionary<string, Robot> robots = new Dictionary<string, Robot>();

    private const string baseTopic = "vrep/pose/robot/";

    // Use this for initialization
    void Start ()
    {
        mMqtt.Connect();
        mMqtt.Subscribe(baseTopic + "+");
        mMqtt.AddCallback(onMqttMessage);
    }

    private void onMqttMessage(object sender, MqttMsgPublishEventArgs e)
    {
        string subTopic = e.Topic.Replace(baseTopic, "");
        string message = System.Text.Encoding.UTF8.GetString(e.Message);
        Debug.Log(subTopic + ": " + message);

        Robot newRobot = JsonUtility.FromJson<Robot>(message.Replace("\n", ""));

        if (robots.ContainsKey(subTopic)) robots[subTopic] = newRobot;
        else robots.Add(subTopic, newRobot);
    }

    // Update is called once per frame
    void Update ()
    {

    }

    public class Robot
    {
        public Vector3 Position;
        public Quaternion Orientation;
    }
}
