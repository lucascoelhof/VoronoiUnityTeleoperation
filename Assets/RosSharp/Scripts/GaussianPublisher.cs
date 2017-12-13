using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class GaussianPublisher : MonoBehaviour
    {
        private RosSocket rosSocket;
        public string topic = "/gaussian";
        public int UpdateTime = 1;
        private int advertizer;
        private GaussianDeformation meshHandler;

        public void Start()
        {
            rosSocket = transform.GetComponent<RosConnector>().RosSocket;
            advertizer = rosSocket.Advertize(topic, "voronoi_hsi/Gaussian");
            meshHandler = this.GetComponent<GaussianDeformation>();
        }

        private void publish(Gaussian message)
        {
            rosSocket.Publish(advertizer, message);
        }

        private void setPublisher(string topic)
        {
            rosSocket.Unadvertize(advertizer);
            advertizer = rosSocket.Advertize(topic, "voronoi_hsi/Gaussian");
        }

        public void Update()
        {
            this.publish(meshHandler.GetGaussian());
        }
    }
}
