using System.Linq;
using UnityEngine;

public class MeshContainerBounds : MonoBehaviour
{
    [Tooltip("Reference to MeshContainer (the object where all walls lie)")]
    public Transform meshContainer;

    public Bounds CalculateBounds()
    {
        if (meshContainer == null)
        {
            Debug.LogWarning("[Bounds] meshContainer did not set!");
            return new Bounds(Vector3.zero, Vector3.one);
        }

        bool firstMesh = true;
        Bounds totalWorldBounds = new Bounds();
        var filters = meshContainer.GetComponentsInChildren<MeshFilter>();
        foreach (var mf in filters)
        {
            Mesh mesh = mf.mesh;
            if (mesh == null) continue;

            Vector3 centerWorld = mf.transform.TransformPoint(mesh.bounds.center);
            Vector3 sizeWorldLocal = mesh.bounds.size;
            Vector3 sizeWorld = new Vector3(
                sizeWorldLocal.x * mf.transform.lossyScale.x,
                sizeWorldLocal.y * mf.transform.lossyScale.y,
                sizeWorldLocal.z * mf.transform.lossyScale.z);

            if (firstMesh)
            {
                totalWorldBounds = new Bounds(centerWorld, sizeWorld);
                firstMesh = false;
            }
            else
            {
                totalWorldBounds.Encapsulate(new Bounds(centerWorld, sizeWorld));
            }
        }
        if (firstMesh)
        {
            Debug.LogWarning("[Bounds] В meshContainer нет активных MeshFilter!");
            return new Bounds(Vector3.zero, Vector3.one);
        }

        Vector3 worldCenterFinal = totalWorldBounds.center;
        Vector3 worldSizeFinal = totalWorldBounds.size;
        Vector3 localCenter = transform.InverseTransformPoint(worldCenterFinal);
        Vector3 lossy = transform.lossyScale;
        Vector3 localSize = new Vector3(
            worldSizeFinal.x / (Mathf.Approximately(lossy.x, 0f) ? 1f : lossy.x),
            worldSizeFinal.y / (Mathf.Approximately(lossy.y, 0f) ? 1f : lossy.y),
            worldSizeFinal.z / (Mathf.Approximately(lossy.z, 0f) ? 1f : lossy.z)
        );

        return new Bounds(localCenter, localSize);
    }
}