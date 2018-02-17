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


using System;
using UnityEngine;


namespace RosSharp.RosBridgeClient
{

    [RequireComponent(typeof(RosConnector))]
    public class FloatSubscriber : MonoBehaviour
    {
        private RosSocket rosSocket;
        public string topic = "";
        public int UpdateTime = 1;
        private double data;

        public void Start()
        {
            rosSocket = transform.GetComponent<RosConnector>().RosSocket;
            rosSocket.Subscribe(topic, "std_msgs/String", updateFloat, UpdateTime);
            Debug.Log("Subscribing to: " + topic);

        }

        private void updateFloat(Message message)
        {
            //StandardFloat64 float64_msg = (StandardFloat64)message;
            //data = float64_msg.data;
            StandardString msg = (StandardString) message;
            data = Convert.ToDouble(msg.data);
        }

        public double getData()
        {
            return data;
        }
    }
}
