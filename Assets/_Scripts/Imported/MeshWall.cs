using UnityEngine;

public class MeshWall
{
    Vector3[] _verts;
    int[] _tris;
    readonly GameObject _wallGO;
    readonly Mesh _mesh;

    public GameObject GameObject => _wallGO;

    public MeshWall(int id, Vector3[] vertices, int[] triangles, Material mat, Transform parent)
    {
        _verts = vertices;
        _tris = triangles;

        _wallGO = new GameObject($"Wall_{id}");
        _wallGO.transform.SetParent(parent, false);

        var filter = _wallGO.AddComponent<MeshFilter>();
        var renderer = _wallGO.AddComponent<MeshRenderer>();
        _mesh = new Mesh();
        filter.mesh = _mesh;

        renderer.sharedMaterial = mat;
        renderer.material.color = Random.ColorHSV();

        ApplyMesh();
    }

    void ApplyMesh()
    {
        _mesh.Clear();
        _mesh.vertices = _verts;
        _mesh.triangles = _tris;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }

    public void UpdateMesh(Vector3[] verts, int[] tris)
    {
        _verts = verts;
        _tris = tris;
        ApplyMesh();
    }
}
