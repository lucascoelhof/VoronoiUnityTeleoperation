# Unity Robot Teleoperation

Create a simulation of robots in Virtual Reality using [Unity3D](https://unity3d.com/).

## How It Works

This application create a bridge between Unity3D and [ROS](http://www.ros.org/), making it possible to simulate a virtual environment with robots. The communication between these two services are made via MQTT and with use of the library [ROS#](https://github.com/siemens/ros-sharp).

The user can interact with the environment and the robots using a Virtual Reality Headset (only Oculus Rift is supported at time).

This setup is ideal to teleoperate robots from distance or simulate reactive swarm algorithms.

## Algorithms Simulated using this project

* [Voronoi Tesselation](https://bitbucket.org/robovr/voronoi_hsi)

## External Dependencies

* [Oculus SDK for Unity](https://developer.oculus.com/) by Facebook
* [ROS-Sharp](https://github.com/siemens/ros-sharp) by Siemens
* [VR Toolkit (VRTK)](https://vrtoolkit.readme.io/) by The Stonefox
* [M2MQTT](https://m2mqtt.wordpress.com/) by Eclipse Paho
