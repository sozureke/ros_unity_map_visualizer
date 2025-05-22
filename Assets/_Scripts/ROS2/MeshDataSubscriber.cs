using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.ArVisualisation;

public class MeshDataSubscriber : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/ar_visualisation/mesh_data";
    public GameObject meshPrefab;
    public float globalMeshScale = 1f;
    private int messageCounter = 0;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<MeshDataMsg>(topicName, OnMeshDataReceived);
    }

    void OnMeshDataReceived(MeshDataMsg msg)
    {
        if (msg.vertices == null || msg.vertices.Length == 0 ||
            msg.triangles == null || msg.triangles.Length == 0) return;

        int uniqueId = messageCounter++;
        GameObject meshObj = Instantiate(meshPrefab);
        meshObj.name = $"Mesh_{uniqueId}";
        ThreeDMap.Instance.RegisterMesh(uniqueId, meshObj);

        var filter = meshObj.GetComponent<MeshFilter>() ?? meshObj.AddComponent<MeshFilter>();

        var verts = new Vector3[msg.vertices.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            var v = msg.vertices[i];
            verts[i] = new Vector3(
                -(float)v.y,
                (float)v.z,
                (float)v.x
            ) * globalMeshScale;
        }

        var mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(msg.triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        filter.mesh = mesh;

        var col = meshObj.GetComponent<MeshCollider>() ?? meshObj.AddComponent<MeshCollider>();
        col.sharedMesh = mesh;
    }
}
