using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UpperView : MonoBehaviour {

    public GameObject vision;
    public VRTK_ControllerEvents controller;

    [Range(12f, 20f)]
    public float altitude = 18f;
    [Range(0.8f, 1.8f)]
    private float speed = 1.6f;

    //private float minSpeed = 0.08f;
    private float defaultHeight;
    private float maxHeight;

    private Boolean returnToOriginalHeight = false;

    // Use this for initialization
    void Start()
    {
        defaultHeight = vision.transform.position.y;
        maxHeight = vision.transform.position.y + altitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.TouchpadPress))
        {
            maxHeight = defaultHeight + altitude;
            if (vision.transform.position.y < maxHeight)
            {
                double relativePosition = maxHeight - vision.transform.position.y;
                float relativeSpeed = Math.Min(4f, Math.Max(speed/1000, (speed * (float)Math.Pow(relativePosition/8, 2))));
                vision.transform.position = new Vector3(vision.transform.position.x, vision.transform.position.y + relativeSpeed, vision.transform.position.z);
                Debug.Log(relativeSpeed);
                returnToOriginalHeight = true;
            }
        }

        else if (returnToOriginalHeight)
        {
            if(vision.transform.position.y > defaultHeight)
            {
                double relativePosition = vision.transform.position.y - defaultHeight + 0.32;
                float relativeSpeed = Math.Min(4f, Math.Max(speed/100, (speed * (float)Math.Pow(relativePosition/5, 2))));
                vision.transform.position = new Vector3(vision.transform.position.x, vision.transform.position.y - relativeSpeed, vision.transform.position.z);
            }

            else
            {
                vision.transform.position = new Vector3(vision.transform.position.x, defaultHeight, vision.transform.position.z);
                returnToOriginalHeight = false;
            }
        }
    }
}
