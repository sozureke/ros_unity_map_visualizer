using UnityEngine;

[DisallowMultipleComponent]
public class PanelFollower : MonoBehaviour
{
    [Tooltip("The transform this panel should follow (MapModeTab)")]
    public Transform target;

    // вычисленные смещения/масштаб при старте
    Vector3 _positionOffset;
    Quaternion _rotationOffset;
    Vector3 _initialScale;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("PanelFollower: target is not set", this);
            enabled = false;
            return;
        }

        // запомним, где мы относительно цели
        _positionOffset = transform.position - target.position;
        _rotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
        _initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        // позиция = позиция цели + наш оффсет
        transform.position = target.position + _positionOffset;

        // вращение: сначала цель, затем наш оффсет
        transform.rotation = target.rotation * _rotationOffset;

        // сохраняем исходный размер
        transform.localScale = _initialScale;
    }
}
