using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    private Vector3 _basePosition;
    private Quaternion _baseRotation;
    [SerializeField] private Vector3 relativePosition;
    private Vector3 _moveRotation;
    private Vector3 _movePosition;
    [SerializeField] private bool fixedAngle;
    private Camera _camera;
    private Transform _transform;
    
    private void Start()
    {
        _camera = Camera.main;
        _transform = GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        _basePosition = _camera.transform.position;

        if (!fixedAngle)
        {
            _baseRotation = _camera.transform.rotation;
        }

        UpdatePosition();
    }

    public void UpdateTransform(Vector3 movement, Vector3 rotation)
    {
        _movePosition += movement * (Time.deltaTime * 0.2f);
        _moveRotation = rotation * Time.deltaTime;
    }

    private void UpdatePosition()
    {
        var rotatedOffset = (_baseRotation * _camera.transform.rotation) * relativePosition;
        var moveOffset = _camera.transform.rotation * _movePosition;

        _transform.SetPositionAndRotation(_basePosition + rotatedOffset + moveOffset, 
            !fixedAngle ? _transform.rotation : _camera.transform.rotation);
        
        _transform.localEulerAngles += _moveRotation;
    }
}