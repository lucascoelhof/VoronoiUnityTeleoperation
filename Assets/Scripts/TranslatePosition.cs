using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatePosition : MonoBehaviour {

    Transform game;

	// Use this for initialization
	void Start () {
        game = this.gameObject.transform;	
	}
	
	// Update is called once per frame
	void Update () {
        //game.position.y = new float(game.position.y-20f);
	}
}
