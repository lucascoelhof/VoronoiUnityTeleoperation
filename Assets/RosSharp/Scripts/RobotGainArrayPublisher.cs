using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class RobotGainArrayPublisher : MonoBehaviour
    {
        private RosSocket rosSocket;
        public string topic = "/voronoi/robot_gain_array";
        private int advertizer;

        public void Start()
        {
            rosSocket = transform.GetComponent<RosConnector>().RosSocket;
            advertizer = rosSocket.Advertize(topic, "voronoi_hsi/RobotGainArray");
        }

        private void publish(List<VoronoiRobotGain> robot_list)
        {
            VoronoiRobotGainArray robotGainArray = new VoronoiRobotGainArray();
            robotGainArray.robot_gain_list = new VoronoiRobotGain[robot_list.Count];
            for(int i=0; i<robot_list.Count; i++)
            {
                robotGainArray.robot_gain_list[i] = robot_list[i];
            }
            rosSocket.Publish(advertizer, robotGainArray);
        }

        private void setPublisher(string topic)
        {
            rosSocket.Unadvertize(advertizer);
            advertizer = rosSocket.Advertize(topic, "voronoi_hsi/RobotGainArray");
        }

        public void Update()
        {

        }
    }
}
