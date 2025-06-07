using System.Collections.Generic;
using UnityEngine;

public class MeshCreator
{
    readonly Transform _parent;
    readonly Material _wallMaterial;
    readonly Dictionary<int, MeshWall> _meshDict = new Dictionary<int, MeshWall>();

    public MeshCreator(Transform parent, Material wallMaterial)
    {
        _parent = parent;
        _wallMaterial = wallMaterial;
    }

    public void UpdateMesh(int id, Vector3[] verts, int[] tris)
    {
        if (_meshDict.TryGetValue(id, out var wall))
        {
            wall.UpdateMesh(verts, tris);
        }
        else
        {
            wall = new MeshWall(id, verts, tris, _wallMaterial, _parent);
            _meshDict[id] = wall;

            if (ThreeDMap.Instance != null)
                ThreeDMap.Instance.RegisterMesh(id, wall.GameObject);
        }
    }

    public void RemoveMesh(int id)
    {
        if (_meshDict.TryGetValue(id, out var wall))
        {
            Object.Destroy(wall.GameObject);
            _meshDict.Remove(id);
            ThreeDMap.Instance?.RemoveMesh(id);
        }
    }

    public bool TryGetMesh(int id, out GameObject go)
    {
        if (_meshDict.TryGetValue(id, out var wall))
        {
            go = wall.GameObject;
            return true;
        }

        go = null;
        return false;
    }
}
