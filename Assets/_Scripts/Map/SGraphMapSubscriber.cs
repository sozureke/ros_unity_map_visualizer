//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Robotics.ROSTCPConnector;
//using RosMessageTypes.LidarSituationalGraphs;

//public class SGraphMapSubscriber : MonoBehaviour
//{
//    [Header("ROS")]
//    [SerializeField] string topic = "/planes_data";

//    [Header("Rendering")]
//    [SerializeField] GameObject planeMarkerPrefab;   
//    [SerializeField] Transform parent;              

//    readonly Dictionary<int, GameObject> markers = new Dictionary<int, GameObject>();

//    void Start()
//    {
//        ROSConnection ros = ROSConnection.GetOrCreateInstance();
//        ros.Subscribe<PlanesDataMsg>(topic, OnMsgReceived);
//    }

//    void OnMsgReceived(PlanesDataMsg msg)
//    {
//        Vector3 pos = new Vector3(
//            (float)msg.floor_center.position.x,
//            (float)msg.floor_center.position.y,
//            (float)msg.floor_center.position.z);

//        Quaternion rot = new Quaternion(
//            (float)msg.floor_center.orientation.x,
//            (float)msg.floor_center.orientation.y,
//            (float)msg.floor_center.orientation.z,
//            (float)msg.floor_center.orientation.w);

//        if (!markers.TryGetValue(msg.id, out GameObject go))
//        {
//            go = Instantiate(planeMarkerPrefab, parent ? parent : transform);
//            go.name = $"Plane_{msg.id}";
//            markers[msg.id] = go;
//        }
//        go.transform.SetPositionAndRotation(pos, rot);
//    }
//}
