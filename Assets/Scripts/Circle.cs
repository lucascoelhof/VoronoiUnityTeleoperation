using UnityEngine;
using System.Collections;
using RosSharp.RosBridgeClient;

[RequireComponent(typeof(LineRenderer))]

public class Circle : MonoBehaviour
{
    private int segments = 50;
    public double radius = 5;
    LineRenderer line;
    public FloatSubscriber floatsubscriber;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        floatsubscriber = gameObject.GetComponent<FloatSubscriber>();

        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        CreatePoints();
    }

    void CreatePoints()
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * (float) radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * (float) radius;

            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }
    }

    private void Update()
    {
        if(floatsubscriber != null)
        {
            radius = floatsubscriber.getData();
            CreatePoints();
        }
    }
}
