using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.ArVisualisation;
using TMPro;

public class MarkerDataSubscriber : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/ar_visualisation/marker_data";
    public GameObject markerPrefab;
    public float globalMeshScale = 1f;
    public float globalMarkerScale = 1f;
    public float heightOffset = 0.2f;
    public float smoothingSpeed = 5f;
    public LayerMask meshLayerMask;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<MarkerDataMsg>(topicName, OnMarkerReceived);
    }

    void OnMarkerReceived(MarkerDataMsg msg)
    {
        if (msg.action == 2)
        {
            ThreeDMap.Instance.RemoveMarker(msg.id);
            ConnectionManager.Instance.RemoveLine(msg.id);
            return;
        }

        if (!ThreeDMap.Instance.TryGetMarker(msg.id, out var marker))
        {
            marker = Instantiate(markerPrefab);
            marker.name = $"Marker_{msg.id}";
            ThreeDMap.Instance.RegisterMarker(msg.id, marker);
        }

        var raw = msg.position;
        var desired = new Vector3(-(float)raw.y, (float)raw.z, (float)raw.x) * globalMeshScale;
        var rayOrigin = desired + Vector3.up * 2f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, 10f, meshLayerMask))
            desired = hit.point;
        else if (ThreeDMap.Instance.TryGetMesh(msg.id, out var meshGO))
        {
            var mf = meshGO.GetComponent<MeshFilter>();
            desired = meshGO.transform.TransformPoint(mf.mesh.bounds.center * globalMeshScale);
        }

        desired += Vector3.up * heightOffset;
        marker.transform.position = Vector3.Lerp(
            marker.transform.position == Vector3.zero ? desired : marker.transform.position,
            desired,
            Time.deltaTime * smoothingSpeed
        );

        var s = msg.marker_scale;
        var rawScale = new Vector3((float)s.x, (float)s.z, (float)s.y) * globalMarkerScale;
        marker.transform.localScale = rawScale;

        var c = msg.marker_color;
        var rend = marker.GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = new Color(c.r, c.g, c.b, c.a);

        var label = marker.GetComponentInChildren<TextMeshPro>();
        if (label != null)
        {
            label.text = $"ID: {msg.id}";
            label.color = new Color(c.r, c.g, c.b, c.a);
            label.transform.localPosition = Vector3.up * (heightOffset + 0.1f);
        }

        if (ThreeDMap.Instance.TryGetMesh(msg.id, out var meshObj))
        {
            var mf = meshObj.GetComponent<MeshFilter>();
            var center = mf.mesh.bounds.center * globalMeshScale;
            var worldCenter = meshObj.transform.TransformPoint(center) + Vector3.up * heightOffset;
            var lr = ConnectionManager.Instance.GetOrCreateLine(msg.id);
            lr.positionCount = 2;
            lr.SetPosition(0, worldCenter);
            lr.SetPosition(1, marker.transform.position);
        }
    }
}
