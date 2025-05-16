using UnityEngine;

[DisallowMultipleComponent]
public class PanelFollower : MonoBehaviour
{
    [Tooltip("The transform this panel should follow (MapModeTab)")]
    public Transform target;

    // ����������� ��������/������� ��� ������
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

        // ��������, ��� �� ������������ ����
        _positionOffset = transform.position - target.position;
        _rotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
        _initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        // ������� = ������� ���� + ��� ������
        transform.position = target.position + _positionOffset;

        // ��������: ������� ����, ����� ��� ������
        transform.rotation = target.rotation * _rotationOffset;

        // ��������� �������� ������
        transform.localScale = _initialScale;
    }
}
