using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }
    public GameObject connectionLinePrefab;

    Dictionary<int, LineRenderer> connections = new Dictionary<int, LineRenderer>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public LineRenderer GetOrCreateLine(int id)
    {
        if (connections.TryGetValue(id, out var lr))
            return lr;

        var go = Instantiate(connectionLinePrefab, transform);
        go.name = $"ConnLine_{id}";
        lr = go.GetComponent<LineRenderer>();
        connections[id] = lr;
        return lr;
    }

    public void RemoveLine(int id)
    {
        if (connections.TryGetValue(id, out var lr))
        {
            Destroy(lr.gameObject);
            connections.Remove(id);
        }
    }
}
