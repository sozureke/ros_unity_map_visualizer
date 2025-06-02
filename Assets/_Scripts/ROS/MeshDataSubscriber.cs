using System.Linq;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.ArVisualisation;

public class MeshDataSubscriber : MonoBehaviour
{
    ROSConnection ros;

    [Header("ROS")]
    public string topicName = "/ar_visualisation/mesh_data";

    [Header("Rendering")]
    public Transform wallParent;  
    public Material wallMaterial;
    public float globalMeshScale = 1f;

    MeshCreator meshCreator;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        meshCreator = new MeshCreator(wallParent, wallMaterial);

        ros.Subscribe<MeshDataMsg>(topicName, OnMesh);          
        Debug.Log($"[MeshSub] subscribed to {topicName}");
    }

    void OnMesh(MeshDataMsg m)
    {
        if (m.vertices == null || m.vertices.Length == 0 ||
            m.triangles == null || m.triangles.Length == 0)
        {
            Debug.LogWarning($"[MeshSub] empty mesh id={m.id}");
            return;
        }

        var rosFirst = m.vertices[0];
        Debug.Log($"[MeshSub] id={m.id} verts={m.vertices.Length} firstROS({rosFirst.x:F2},{rosFirst.y:F2},{rosFirst.z:F2})");

        var verts = m.vertices
                     .Select(v => RosMessageConverter.Vector3MsgToUnity(v) * globalMeshScale)
                     .ToArray();

        Debug.Log($"[MeshSub] id={m.id} firstUnity({verts[0].x:F2},{verts[0].y:F2},{verts[0].z:F2})");

        meshCreator.UpdateMesh(m.id, verts, m.triangles);
    }
}
