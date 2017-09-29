using UnityEngine;
using UnityEditor;

public class OculusButtons : ScriptableObject
{
    //> in Buttons <
    //Left:  left_index_trigger, left_hand_trigger, X, Y
    //       left_thumbstick, left_thumb
    //Right: right_index_trigger, right_hand_trigger, A, B
    //       right_thumbstick, right_thumb

    public static TouchController touchController { get; set; }

    public static void Update()
    {
        OVRInput.Update();

        TouchController newState = new TouchController();

        newState.A.state = OVRInput.Get(OVRInput.Button.One);
        newState.A.eventDown = OVRInput.GetDown(OVRInput.Button.One);
        newState.A.eventUp = OVRInput.GetUp(OVRInput.Button.One);

        newState.B.state = OVRInput.Get(OVRInput.Button.Two);
        newState.B.eventDown = OVRInput.GetDown(OVRInput.Button.Two);
        newState.B.eventUp = OVRInput.GetUp(OVRInput.Button.Two);

        newState.X.state = OVRInput.Get(OVRInput.Button.Three);
        newState.X.eventDown = OVRInput.GetDown(OVRInput.Button.Three);
        newState.X.eventUp = OVRInput.GetUp(OVRInput.Button.Three);

        newState.Y.state = OVRInput.Get(OVRInput.Button.Four);
        newState.Y.eventDown = OVRInput.GetDown(OVRInput.Button.Four);
        newState.Y.eventUp = OVRInput.GetUp(OVRInput.Button.Four);

        newState.LThumbstickButton.state = OVRInput.Get(OVRInput.Button.PrimaryThumbstick);
        newState.LThumbstickButton.eventDown = OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick);
        newState.LThumbstickButton.eventUp = OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick);

        newState.RThumbstickButton.state = OVRInput.Get(OVRInput.Button.SecondaryThumbstick);
        newState.RThumbstickButton.eventDown = OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick);
        newState.RThumbstickButton.eventUp = OVRInput.GetUp(OVRInput.Button.SecondaryThumbstick);

        newState.LIndexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        newState.RIndexTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        newState.LHandTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        newState.RHandTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);

        newState.LThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        newState.RThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);


        touchController = newState;
    }

    public struct TouchController
    {
        public Button A;
        public Button B;
        public Button X;
        public Button Y;
        public Button LThumbstickButton;
        public Button RThumbstickButton;
        public float LIndexTrigger;
        public float RIndexTrigger;
        public float LHandTrigger;
        public float RHandTrigger;
        public Vector2 LThumbstick;
        public Vector2 RThumbstick;
    }

    public struct Button
    {
        public bool state;
        public bool eventUp;
        public bool eventDown;
    }

}