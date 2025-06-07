using UnityEngine;

public class MeshWall
{
    Vector3[] _verts;
    int[] _tris;
    readonly GameObject _wallGO;
    readonly Mesh _mesh;

    public GameObject GameObject => _wallGO;
    public Mesh Mesh => _mesh;
    public Bounds Bounds => _mesh.bounds;

    public MeshWall(int id, Vector3[] vertices, int[] triangles, Material mat, Transform parent)
    {
        _verts = vertices;
        _tris = triangles;
        _wallGO = new GameObject($"Wall_{id}");
        _wallGO.layer = LayerMask.NameToLayer("MapFor2D");
        _wallGO.transform.SetParent(parent, true);

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

    public Vector3 GetCenterWorld()
    {
        return _wallGO.transform.TransformPoint(_mesh.bounds.center);
    }
}
