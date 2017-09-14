# unity-robot-teleoperation

## Handling Oculus VR sensor

A class ``Ts`` was created to handle only the sensors data required. A object must be created and the sensors data assigned to the object. The object then will be parsed using _JSON Serialization_. Unity has a built-in JSON Serialization as follow:

```
string json = JsonUtility.ToJson(object);
```
