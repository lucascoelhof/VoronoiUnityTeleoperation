using System.Collections;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine;

public class MQTT : MonoBehaviour {

    public MqttClient Client;
    //private const string DEFAULT_HOSTNAME = "150.164.212.253";
    private const string DEFAULT_HOSTNAME = "iot.eclipse.org";

    private const int DEFAULT_QoS = 0;
    public string message;


    /// <summary>
    /// Connect to a MQTT Broker (if no parameter is given, set to default broker)
    /// </summary>
    /// <param name="hostname">Broker Hostname</param>
    public void Connect(string hostname = DEFAULT_HOSTNAME)
    {
        Client = new MqttClient(hostname);
        Client.MqttMsgPublishReceived += onMqttMessage;
        string clientId = System.Guid.NewGuid().ToString();
        Client.Connect(clientId);
        Debug.Log("Connecting to host " + hostname + " as " + clientId);
    }

    /// <summary>
    /// Publish message to specific topic
    /// </summary>
    /// <param name="topic">Topic to publish into</param>
    /// <param name="message">Message to publish</param>
    /// <param name="qos">QoS(0, 1, 2)</param>
    public void Publish(string topic, string message, int qos = 0)
    {
        byte qos_type;
        
        if (qos == 1)
            qos_type = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
        else if (qos == 2)
            qos_type = MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE;
        else
            qos_type = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE;

        Client.Publish(topic, Encoding.UTF8.GetBytes(message), qos_type, false);
    }

    /// <summary>
    /// Subscribe to a single topic (MQTT.message keeps the last received message)
    /// </summary>
    /// <param name="topic">Topic to subscribe</param>
    /// <param name="qos">QoS(0, 1, 2)</param>
    public void Subscribe(string topic, int qos = DEFAULT_QoS)
    {
        Subscribe(new string[] { topic }, qos);
    }

    /// <summary>
    /// Subscribe to array of topics (MQTT.message keeps the last received message)
    /// </summary>
    /// <param name="topics">Topics to subscribe</param>
    /// <param name="qos">QoS(0, 1, 2)</param>
    public void Subscribe(string[] topics, int qos = DEFAULT_QoS)
    {
        byte[] qos_type;

        if (qos == 1)
            qos_type = new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
        else if (qos == 2)
            qos_type = new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
        else
            qos_type = new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE };

        Client.Subscribe(topics, qos_type);
    }

    private void onMqttMessage(object sender, MqttMsgPublishEventArgs e)
    {
        message = System.Text.Encoding.UTF8.GetString(e.Message);
        Debug.Log("Received message from Broker: " + message);
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
