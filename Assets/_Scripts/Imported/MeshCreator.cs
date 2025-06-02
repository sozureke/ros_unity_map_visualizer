using UnityEngine;

public class MeshCreator
{
    readonly Transform _parent;
    readonly Material _wallMaterial;

    public MeshCreator(Transform parent, Material wallMaterial)
    {
        _parent = parent;
        _wallMaterial = wallMaterial;
    }

    public void UpdateMesh(int id, Vector3[] verts, int[] tris)
    {
        if (!ThreeDMap.Instance.TryGetMesh(id, out var go))
        {
            var wall = new MeshWall(id, verts, tris, _wallMaterial, _parent);
            ThreeDMap.Instance.RegisterMesh(id, wall.GameObject);
            return;
        }

        go.GetComponent<MeshWall>().UpdateMesh(verts, tris);
    }
}
