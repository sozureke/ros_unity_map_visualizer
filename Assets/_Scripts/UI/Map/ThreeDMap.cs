using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThreeDMap : MonoBehaviour
{
    public Transform meshContainer;
    public Transform markerContainer;

    private readonly Dictionary<int, GameObject> meshes = new Dictionary<int, GameObject>();
    private readonly Dictionary<int, GameObject> markers = new Dictionary<int, GameObject>();

    public static ThreeDMap Instance { get; private set; }

    public event Action<int> MeshAdded;
    public event Action<int> MeshRemoved;
    public event Action<int> MarkerAdded;
    public event Action<int> MarkerRemoved;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (meshContainer == null || markerContainer == null)
            Debug.LogError("ThreeDMap: meshContainer or markerContainer is not assigned", this);
    }

    public void RegisterMesh(int id, GameObject go)
    {
        meshes[id] = go;
        go.transform.SetParent(meshContainer, false);
        MeshAdded?.Invoke(id);
    }

    public bool TryGetMesh(int id, out GameObject go) => meshes.TryGetValue(id, out go);

    public void RemoveMesh(int id)
    {
        if (meshes.TryGetValue(id, out var old))
        {
            Destroy(old);
            meshes.Remove(id);
            MeshRemoved?.Invoke(id);
        }
    }

    public void RegisterMarker(int id, GameObject go)
    {
        markers[id] = go;
        go.transform.SetParent(markerContainer, false);
        MarkerAdded?.Invoke(id);
    }

    public bool TryGetMarker(int id, out GameObject go) => markers.TryGetValue(id, out go);

    public void RemoveMarker(int id)
    {
        if (markers.TryGetValue(id, out var old))
        {
            Destroy(old);
            markers.Remove(id);
            MarkerRemoved?.Invoke(id);
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

    public IReadOnlyCollection<GameObject> GetAllMeshes() => meshes.Values;
    public IReadOnlyCollection<GameObject> GetAllMarkers() => markers.Values;

    public void ShowRawMeshes(bool show)
    {
        if (meshContainer != null)
            meshContainer.gameObject.SetActive(show);
    }

    public void SetVisible(bool visible) => gameObject.SetActive(visible);
}
