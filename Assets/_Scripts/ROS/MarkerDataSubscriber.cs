using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.ArVisualisation;
using TMPro;

public class MarkerDataSubscriber : MonoBehaviour
{
    ROSConnection ros;

    [Header("ROS")]
    public string topicName = "/ar_visualisation/marker_data";

    [Header("Prefab / Parent")]
    public GameObject markerPrefab;
    public Transform markerParent;


    [Header("Visual params")]
    [Range(0.015f, 1f)]
    public float globalMarkerScale = 0.5f;
    public float heightOffset = 0.2f;
    public float smoothingSpeed = 5f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<MarkerDataMsg>(topicName, OnMarkerReceived);
    }

    void OnMarkerReceived(MarkerDataMsg msg)
    {
        if (msg.action == 2)
        {
            ThreeDMap.Instance?.RemoveMarker(msg.id);
            ConnectionManager.Instance?.RemoveLine(msg.id);
            return;
        }

        if (markerPrefab == null || ThreeDMap.Instance == null || ConnectionManager.Instance == null)
            return;

        Transform parent = markerParent != null ? markerParent : ThreeDMap.Instance.markerContainer;
        if (parent == null) return;

        GameObject marker;
        if (!ThreeDMap.Instance.TryGetMarker(msg.id, out marker))
        {
            marker = Instantiate(markerPrefab);
            marker.name = $"Marker_{msg.id}";
            int layer = LayerMask.NameToLayer("MapFor2D");
            marker.layer = layer;
            foreach (Transform c in marker.GetComponentsInChildren<Transform>())
                c.gameObject.layer = layer;
            ThreeDMap.Instance.RegisterMarker(msg.id, marker);
        }

        marker.transform.SetParent(parent, true);

        Vector3 worldPos = RosMessageConverter.PointMsgToUnity(msg.position) + Vector3.up * heightOffset;
        Vector3 current = marker.transform.position;
        marker.transform.position = current == Vector3.zero ? worldPos : Vector3.Lerp(current, worldPos, Time.deltaTime * smoothingSpeed);

        Vector3 scaleMsg = RosMessageConverter.Vector3MsgToUnity(msg.marker_scale);
        marker.transform.localScale = new Vector3(scaleMsg.x, scaleMsg.z, scaleMsg.y) * globalMarkerScale;

        Color color = RosMessageConverter.ColorMsgToUnity(msg.marker_color);
        Renderer rend = marker.GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = color;

        TextMeshPro label = marker.GetComponentInChildren<TextMeshPro>();
        if (label != null)
        {
            label.text = $"ID: {msg.id}";
            label.color = color;
            label.transform.localPosition = Vector3.up * (heightOffset + 0.1f);
        }

        if (ThreeDMap.Instance.TryGetMesh(msg.id, out var meshObj))
        {
            MeshFilter mf = meshObj.GetComponent<MeshFilter>();
            if (mf != null)
            {
                Vector3 worldCenter = meshObj.transform.TransformPoint(mf.mesh.bounds.center) + Vector3.up * heightOffset;
                LineRenderer lr = ConnectionManager.Instance.GetOrCreateLine(msg.id);
                lr.positionCount = 2;
                lr.SetPosition(0, worldCenter);
                lr.SetPosition(1, marker.transform.position);
            }
        }
    }
}
