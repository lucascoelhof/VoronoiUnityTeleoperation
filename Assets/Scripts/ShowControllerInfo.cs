using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ShowControllerInfo : MonoBehaviour {

    public VRTK_ControllerEvents controllerLeft;

    public GameObject tooltipLeft;
    public GameObject tooltipRight;

    public GameObject helperShow;
    public GameObject helperHide;

    //private bool showInfo = false;
    private enum LastState { on, off };
    private LastState buttonState = LastState.on;

    // Use this for initialization
    void Start () {
        tooltipLeft.SetActive(true);
        tooltipRight.SetActive(true);
        helperHide.SetActive(true);
        helperShow.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (controllerLeft.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.StartMenuPress))
        {
            if (buttonState == LastState.off)
            {
                if (tooltipLeft.activeSelf)
                {
                    tooltipLeft.SetActive(false);
                    tooltipRight.SetActive(false);
                    helperHide.SetActive(false);
                    helperShow.SetActive(true);
                }
                else
                {
                    tooltipLeft.SetActive(true);
                    tooltipRight.SetActive(true);
                    helperShow.SetActive(false);
                    helperHide.SetActive(true);
                }
                buttonState = LastState.on;
            }
        }
        else
        {
            buttonState = LastState.off;
        }
	}
}
