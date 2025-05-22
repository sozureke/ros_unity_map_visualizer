using System.Collections.Generic;
using UnityEngine;

public class ThreeDMap : MonoBehaviour
{
    public Transform meshContainer;
    public Transform markerContainer;

    private readonly Dictionary<int, GameObject> meshes = new Dictionary<int, GameObject>();
    private readonly Dictionary<int, GameObject> markers = new Dictionary<int, GameObject>();


    public static ThreeDMap Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void RegisterMesh(int id, GameObject go)
    {
        meshes[id] = go;
        go.transform.SetParent(meshContainer, false);
    }
    public bool TryGetMesh(int id, out GameObject go) => meshes.TryGetValue(id, out go);
    public void RemoveMesh(int id)
    {
        if (meshes.TryGetValue(id, out var old))
        {
            Destroy(old);
            meshes.Remove(id);
        }
    }

    public void RegisterMarker(int id, GameObject go)
    {
        markers[id] = go;
        go.transform.SetParent(markerContainer, false);
    }
    public bool TryGetMarker(int id, out GameObject go) => markers.TryGetValue(id, out go);
    public void RemoveMarker(int id)
    {
        if (markers.TryGetValue(id, out var old))
        {
            Destroy(old);
            markers.Remove(id);
        }
    }

    public void ClearAll()
    {
        foreach (Transform c in meshContainer) Destroy(c.gameObject);
        foreach (Transform c in markerContainer) Destroy(c.gameObject);
        meshes.Clear();
        markers.Clear();
    }

    public int MeshCount() => meshes.Count;
    public int MarkerCount() => markers.Count;

    public void SetVisible(bool visible) => gameObject.SetActive(visible);
}
