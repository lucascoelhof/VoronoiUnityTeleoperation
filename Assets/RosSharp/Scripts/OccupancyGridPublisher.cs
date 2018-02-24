/*
© Federal Univerity of Minas Gerais (Brazil), 2017
Author: Lucas Coelho Figueiredo (me@lucascoelho.net)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>.

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class OccupancyGridPublisher : MonoBehaviour
    {
        private RosSocket rosSocket;
        public string topic = "/voronoi/map";
        private int advertizer;
        public GameObject UnityGameObject;
        private OccupancyGridManager occGridManager;

        private void Start()
        {
            rosSocket = transform.GetComponent<RosConnector>().RosSocket;
            advertizer = rosSocket.Advertize(topic, "nav_msgs/OccupancyGrid");
            if (UnityGameObject != null)
                occGridManager = UnityGameObject.GetComponent<OccupancyGridManager>();
            setPublisher(this.topic);
        }

        public void publish(NavigationOccupancyGrid message)
        {
            rosSocket.Publish(advertizer, message);
            Debug.Log("Published occ grid");
        }

        private void setPublisher(string topic)
        {
            rosSocket.Unadvertize(advertizer);
            advertizer = rosSocket.Advertize(topic, "nav_msgs/OccupancyGrid");
        }

        private void Update()
        {

        }
    }
}
