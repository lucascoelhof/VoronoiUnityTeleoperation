using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class VideoStream : MonoBehaviour {

    public String sourceURL;
    public enum eyeType
    {
        left=0,
        right=1
    };

    public eyeType eye;

    public MeshRenderer frame;    //Mesh for displaying video

    //http://150.164.212.253:8080/stream?topic=/camera1/image&quality=40 
    //http://150.164.212.253:8080/stream?topic=/camera2/image&quality=40

    //private static Texture2D textureLeft = new Texture2D(2,2);
    //private static Texture2D textureRight = new Texture2D(2, 2);
    private Stream stream;

    public GameObject ImageOnPanel;  ///set this in the inspector
    private RawImage rawImage;

    RenderTexture rt;


    public void GetVideo()
    {
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
        OculusTeleoperation.streamingVideo[(int)eye] = true;

        Byte[] JpegData = new Byte[65536];

        while (true)
        {
            int bytesToRead = FindLength(stream);
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

            MemoryStream ms = new MemoryStream(JpegData, 0, bytesToRead, false, true);
            //texture.LoadImage(ms.GetBuffer());
            //if (eye == eyeType.left)
            OculusTeleoperation.textureLeft.LoadImage(ms.GetBuffer());
            //if (eye == eyeType.right) textureRight.LoadImage(ms.GetBuffer());
            //frame.material.mainTexture = texture;
            stream.ReadByte(); // CR after bytes

            OculusTeleoperation.streamingVideo[(int)eye] = false;
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

    //IEnumerator CameraToTexture()
    //{
    //    OculusTeleoperation.renderingTexture[(int)eye] = true;

    //    //texture = new Texture2D(2, 2);
    //    rawImage = (RawImage)this.GetComponent<RawImage>();
    //    rawImage.texture = texture;

    //    OculusTeleoperation.renderingTexture[(int)eye] = false;

    //    yield return null;
    //}

    // Use this for initialization
    void Start () {
        //texture = new Texture2D(2, 2);
        //img = (RawImage) ImageOnPanel.GetComponent<RawImage>();
        //img.texture = (Texture) texture;
        //texture.Apply();
        //var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

        //if(!OculusTeleoperation.renderingTexture[(int)eye])
        //    StartCoroutine(CameraToTexture());

        if (!OculusTeleoperation.streamingVideo[(int)eye])
        {
            //texture = new Texture2D(2, 2);
            OculusTeleoperation.textureLeft = new Texture2D(2, 2);
            rawImage = (RawImage)this.GetComponent<RawImage>();
            //rawImage.texture = texture;
            //if (eye == eyeType.left)
            rawImage.texture = OculusTeleoperation.textureLeft;
        }
        //if (eye == eyeType.right) rawImage.texture = textureRight;

        //Debug.Log(OculusTeleoperation.streamingVideo[(int)eye]);
        //if(!OculusTeleoperation.streamingVideo[(int)eye])
        GetVideo();
    }

    // Update is called once per frame
    void Update () {
    }

    /*void OnGUI()
    {
        if (eye == eyeType.left)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width / 2, Screen.height), texture);
        }
        else
        {
            GUI.DrawTexture(new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height), texture);
        }

    } */

} 
