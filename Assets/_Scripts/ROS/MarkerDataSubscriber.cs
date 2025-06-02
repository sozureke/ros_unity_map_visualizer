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
    public float globalMarkerScale = 1f;
    public float heightOffset = 0.2f;
    public float smoothingSpeed = 5f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<MarkerDataMsg>(topicName, OnMarkerReceived);
        Debug.Log($"[MarkerSub] Subscribed to topic {topicName}");
    }

    void OnMarkerReceived(MarkerDataMsg msg)
    {
        Debug.Log($"[MarkerSub] Received msg → id={msg.id}, action={msg.action}, " +
                  $"position=({msg.position.x:F2},{msg.position.y:F2},{msg.position.z:F2}), " +
                  $"scale=({msg.marker_scale.x:F2},{msg.marker_scale.y:F2},{msg.marker_scale.z:F2}), " +
                  $"color=({msg.marker_color.r:F2},{msg.marker_color.g:F2},{msg.marker_color.b:F2},{msg.marker_color.a:F2}), " +
                  $"linesCount={(msg.lines != null ? msg.lines.Length : 0)}");

        Transform parent = markerParent != null
            ? markerParent
            : (ThreeDMap.Instance != null ? ThreeDMap.Instance.markerContainer : null);

        if (markerPrefab == null)
        {
            Debug.LogWarning("[MarkerSub] markerPrefab is not assigned!");
            return;
        }
        if (parent == null)
        {
            Debug.LogWarning("[MarkerSub] markerParent or ThreeDMap.Instance.markerContainer is null!");
            return;
        }
        if (ThreeDMap.Instance == null)
        {
            Debug.LogWarning("[MarkerSub] ThreeDMap.Instance is null!");
            return;
        }
        if (ConnectionManager.Instance == null)
        {
            Debug.LogWarning("[MarkerSub] ConnectionManager.Instance is null!");
            return;
        }

        if (msg.action == 2)
        {
            Debug.Log($"[MarkerSub] Removing marker id={msg.id}");
            ThreeDMap.Instance.RemoveMarker(msg.id);
            ConnectionManager.Instance.RemoveLine(msg.id);
            return;
        }

        GameObject marker;
        if (!ThreeDMap.Instance.TryGetMarker(msg.id, out marker))
        {
            marker = Instantiate(markerPrefab, parent);
            marker.name = $"Marker_{msg.id}";
            ThreeDMap.Instance.RegisterMarker(msg.id, marker);
            Debug.Log($"[MarkerSub] Created new marker id={msg.id}");
        }
        else
        {
            Debug.Log($"[MarkerSub] Reusing existing marker id={msg.id}");
        }

        Vector3 worldPos = RosMessageConverter.PointMsgToUnity(msg.position) + Vector3.up * heightOffset;
        Vector3 current = marker.transform.position;
        if (current == Vector3.zero)
        {
            marker.transform.position = worldPos;
        }
        else
        {
            marker.transform.position = Vector3.Lerp(current, worldPos, Time.deltaTime * smoothingSpeed);
        }
        Debug.Log($"[MarkerSub] Marker {msg.id} moved to {marker.transform.position}");


        Vector3 scaleMsg = RosMessageConverter.Vector3MsgToUnity(msg.marker_scale);
        Vector3 unityScale = new Vector3(scaleMsg.x, scaleMsg.z, scaleMsg.y) * globalMarkerScale;
        marker.transform.localScale = unityScale;
        Debug.Log($"[MarkerSub] Marker {msg.id} scale set to {unityScale}");

        Color colorMsg = RosMessageConverter.ColorMsgToUnity(msg.marker_color);
        Renderer rend = marker.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material.color = colorMsg;
            Debug.Log($"[MarkerSub] Marker {msg.id} color set to {colorMsg}");
        }

        TextMeshPro label = marker.GetComponentInChildren<TextMeshPro>();
        if (label != null)
        {
            label.text = $"ID: {msg.id}";
            label.color = colorMsg;
            label.transform.localPosition = Vector3.up * (heightOffset + 0.1f);
            Debug.Log($"[MarkerSub] Marker {msg.id} label updated");
        }

        if (ThreeDMap.Instance.TryGetMesh(msg.id, out var meshObj))
        {
            MeshFilter mf = meshObj.GetComponent<MeshFilter>();
            if (mf != null)
            {
                Vector3 center = mf.mesh.bounds.center;
                Vector3 worldCenter = meshObj.transform.TransformPoint(center) + Vector3.up * heightOffset;
                LineRenderer lr = ConnectionManager.Instance.GetOrCreateLine(msg.id);
                lr.positionCount = 2;
                lr.SetPosition(0, worldCenter);
                lr.SetPosition(1, marker.transform.position);
                Debug.Log($"[MarkerSub] Updated line for marker id={msg.id}");
            }
        }
    }
}
