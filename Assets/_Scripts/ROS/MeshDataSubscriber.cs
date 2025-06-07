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

    [Range(0.015f, 1f)]
    public float globalMeshScale = 0.5f;

    MeshCreator meshCreator;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        meshCreator = new MeshCreator(wallParent, wallMaterial);
        ros.Subscribe<MeshDataMsg>(topicName, OnMesh);
    }

    void OnMesh(MeshDataMsg m)
    {
        if (m.vertices == null || m.vertices.Length == 0 ||
            m.triangles == null || m.triangles.Length == 0)
            return;

        var verts = m.vertices
            .Select(v => RosMessageConverter.Vector3MsgToUnity(v) * globalMeshScale)
            .ToArray();

        meshCreator.UpdateMesh(m.id, verts, m.triangles);
    }
}
