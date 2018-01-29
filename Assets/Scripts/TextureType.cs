using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureType : MonoBehaviour {

    enum filters {point=FilterMode.Point, bilinear=FilterMode.Bilinear, trilinear=FilterMode.Trilinear};

    public FilterMode filterMode;

    private FilterMode texture;

	// Use this for initialization
	void Start () {
        texture = GetComponent<Renderer>().material.mainTexture.filterMode;
        texture = filterMode;
	}
	
	// Update is called once per frame
	void Update () {
    }
}
