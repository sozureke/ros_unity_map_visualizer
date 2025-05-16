using UnityEngine;

public class MapTabFollower : MonoBehaviour
{
    [Tooltip("Panel to follow")]
    public Transform panel;

    [Tooltip("Offset in panel's local space")]
    public Vector3 localOffset = new Vector3(0f, 0.5f, 0f);

    void LateUpdate()
    {
        transform.position = panel.TransformPoint(localOffset);
        transform.rotation = panel.rotation;
    }
}
