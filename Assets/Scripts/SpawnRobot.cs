using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRobot : MonoBehaviour {

    GameObject epuck;

    public Vector3 Position;
    public Quaternion Orientation;

    // Use this for initialization
    void Start () {
        epuck = Instantiate(Resources.Load("epuck"), new Vector3(0, 0, 0), Quaternion.Euler(0, 180, 0)) as GameObject;
    }
	
	// Update is called once per frame
	void Update () {
        Destroy(epuck);
        epuck = Instantiate(Resources.Load("epuck"), Position, Orientation) as GameObject;
    }
}
