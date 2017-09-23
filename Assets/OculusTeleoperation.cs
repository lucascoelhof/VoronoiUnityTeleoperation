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

    private const string HOSTNAME = "iot.eclipse.org";

    public MeshRenderer frame;    //Mesh for displaying video

    //http://150.164.212.253:8080/stream?topic=/camera1/image&quality=40
    //http://150.164.212.253:8080/stream?topic=/camera2/image&quality=40

    private string sourceURL = "http://24.172.4.142/mjpg/video.mjpg";
    private string source2URL = "http://24.172.4.142/mjpg/video.mjpg";
    private Texture2D texture;
    private Texture2D texture2;
    private Stream stream;
    private Stream stream2;
    private Canvas canvas;

    public Transform leftHand { get; private set; }
    public Transform rightHand { get; private set; }
    public Transform leftEye { get; private set; }
    public Transform rightEye { get; private set; }
    public Transform head { get; private set; }

    [SerializeField]
    protected OVRInput.Controller m_controller;

    [SerializeField]
    protected Transform m_parentTransform;

    protected Vector3 m_anchorOffsetPosition;
    protected Quaternion m_anchorOffsetRotation;



    private int mFrameRefresh = 0;

    Byte[] bufferData = new Byte[65536];

    void MqttConnect(string hostname) {
        mMqttClient = new MqttClient(hostname);
        mMqttClient.MqttMsgPublishReceived += onMqttMessage;
        string clientId = System.Guid.NewGuid().ToString();
        mMqttClient.Connect(clientId);
    }

    void UpdatePose() {


        //bool monoscopic = OVRManager.instance.monoscopic;
        //head.localRotation = UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye);
        //leftEye.localRotation = monoscopic ? head.localRotation : VR.InputTracking.GetLocalRotation(VR.VRNode.LeftEye);
        //Debug.Log(UnityEngine.VR.VRNode.LeftEye);
        //leftEye.localRotation = UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.LeftEye);
        //rightEye.localRotation = monoscopic ? head.localRotation : VR.InputTracking.GetLocalRotation(VR.VRNode.RightEye);
        //leftHand.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
        //rightHand.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        //head.localPosition = VR.InputTracking.GetLocalPosition(VR.VRNode.CenterEye);
        //leftEye.localPosition = monoscopic ? head.localPosition : VR.InputTracking.GetLocalPosition(VR.VRNode.LeftEye);
        //rightEye.localPosition = monoscopic ? head.localPosition : VR.InputTracking.GetLocalPosition(VR.VRNode.RightEye);
        //leftHand.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        //rightHand.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        //OVRInput.Update();
        //bool lTrigger = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);
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

        texture2 = new Texture2D(2, 2);
        req = (HttpWebRequest)WebRequest.Create(source2URL);
        resp = req.GetResponse();
        stream2 = resp.GetResponseStream();
        StartCoroutine(GetFrame());
    }

    IEnumerator GetFrame()
    {
        Byte[] JpegData = new Byte[65536];
        Byte[] JpegData2 = new Byte[65536];

        while (true)
        {
            int bytesToRead = FindLength(stream);
            //print(bytesToRead);
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

            leftToRead = bytesToRead;

            while (leftToRead > 0)
            {
                leftToRead -= stream2.Read(JpegData2, bytesToRead - leftToRead, leftToRead);
                yield return null;
            }

            MemoryStream ms = new MemoryStream(JpegData, 0, bytesToRead, false, true);
            MemoryStream ms2 = new MemoryStream(JpegData, 0, bytesToRead, false, true);

            texture.LoadImage(ms.GetBuffer());
            texture2.LoadImage(ms2.GetBuffer());
            //frame.material.mainTexture = texture;
            stream.ReadByte(); // CR after bytes
            stream2.ReadByte(); // LF after bytes
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
            if (b == 10)
                continue; // ignore LF char
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

    IEnumerator LoadDevice(string newDevice)
    {
        VRSettings.LoadDeviceByName(newDevice);
        yield return null;
        VRSettings.enabled = true;
    }

    public void PrintSensors ()
    {
        UpdatePose();
        Debug.Log(head.localRotation);
    }

    // Use this for initialization
    void Start () {
        MqttConnect(HOSTNAME);
        StartCoroutine(LoadDevice("Split"));
        //canvas = 
        GetVideo();
        PrintSensors();

        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;

        if (m_parentTransform == null)
        {
            if (gameObject.transform.parent != null)
            {
                m_parentTransform = gameObject.transform.parent.transform;
            }
            else
            {
                m_parentTransform = new GameObject().transform;
                m_parentTransform.position = Vector3.zero;
                m_parentTransform.rotation = Quaternion.identity;
            }
        }
    }

    protected virtual void Awake()
    {
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;

        // If we are being used with an OVRCameraRig, let it drive input updates, which may come from Update or FixedUpdate.
    }

    // Update is called once per frame
    void Update () {
        if((mFrameRefresh++)%60 == 0) {
            //MqttPublish("lucas_teste_unity_oculus", DateTime.Now.ToString("h:mm:ss tt"), 0);
            UpdatePose();
        }
        
	}

    public void OnGUI() {
        GUI.DrawTexture(new Rect(0, 0, Screen.width/2, Screen.height), texture);
        GUI.DrawTexture(new Rect(Screen.width/2, 0, Screen.width/2, Screen.height), texture);
    }
}
