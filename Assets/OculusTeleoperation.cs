using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class OculusTeleoperation : MonoBehaviour {

    private MqttClient mMqttClient;

    private const string HOSTNAME = "iot.eclipse.org";

    public MeshRenderer frame;    //Mesh for displaying video

    private string sourceURL = "http://24.172.4.142/mjpg/video.mjpg";
    private string source2URL = "http://24.172.4.142/mjpg/video.mjpg";
    private Texture2D texture;
    private Texture2D texture2;
    private Stream stream;

    private int mFrameRefresh = 0;

    Byte[] bufferData = new Byte[65536];

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

    public void GetVideo()
    {
        texture = new Texture2D(2, 2);
        // create HTTP request
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sourceURL);
        //Optional (if authorization is Digest)
        //req.Credentials = new NetworkCredential("username", "password");
        // get response
        WebResponse resp = req.GetResponse();
        // get response stream
        stream = resp.GetResponseStream();
        StartCoroutine(GetFrame());
    }

    IEnumerator GetFrame()
    {
        Byte[] JpegData = new Byte[65536];

        while (true)
        {
            int bytesToRead = FindLength(stream);
            print(bytesToRead);
            if (bytesToRead == -1)
            {
                print("End of stream");
                yield break;
            }

            int leftToRead = bytesToRead;

            while (leftToRead > 0)
            {
                leftToRead -= stream.Read(JpegData, bytesToRead - leftToRead, leftToRead);
                yield return null;
            }

            MemoryStream ms = new MemoryStream(JpegData, 0, bytesToRead, false, true);

            texture.LoadImage(ms.GetBuffer());
            //frame.material.mainTexture = texture;
            stream.ReadByte(); // CR after bytes
            stream.ReadByte(); // LF after bytes
        }
    }

    int FindLength(Stream stream)
    {
        int b;
        string line = "";
        int result = -1;
        bool atEOL = false;

        while ((b = stream.ReadByte()) != -1)
        {
            if (b == 10) continue; // ignore LF char
            if (b == 13)
            { // CR
                if (atEOL)
                {  // two blank lines means end of header
                    stream.ReadByte(); // eat last LF
                    return result;
                }
                if (line.StartsWith("Content-Length:"))
                {
                    result = Convert.ToInt32(line.Substring("Content-Length:".Length).Trim());
                }
                else
                {
                    line = "";
                }
                atEOL = true;
            }
            else
            {
                atEOL = false;
                line += (char)b;
            }
        }
        return -1;
    }

    // Use this for initialization
    void Start () {
        MqttConnect(HOSTNAME);
        GetVideo();
    }
	
	// Update is called once per frame
	void Update () {
        if((mFrameRefresh++)%60 == 0) {
            MqttPublish("lucas_teste_unity_oculus", DateTime.Now.ToString("h:mm:ss tt"), 0);
        }
	}

    public void OnGUI() {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
    }
}
