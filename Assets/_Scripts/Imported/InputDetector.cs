using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class InputDetector : MonoBehaviour
{
    [SerializeField] private GameObject mapObject;
    private PositionHandler _positionHandler;
    private ROSConnection _ros;
    public string twistTopic = "cmd_vel";
    private bool _mapControlMode = true;
    private readonly Dictionary<KeyCode, bool> _inputs = new Dictionary<KeyCode, bool>
    {
        { KeyCode.Tab,        false },
        { KeyCode.Space,      false },
        { KeyCode.LeftShift,  false },
        { KeyCode.W,          false },
        { KeyCode.A,          false },
        { KeyCode.S,          false },
        { KeyCode.D,          false },
        { KeyCode.UpArrow,    false },
        { KeyCode.DownArrow,  false },
        { KeyCode.LeftArrow,  false },
        { KeyCode.RightArrow, false },
    };

    private void Awake()
    {
        _positionHandler = mapObject.GetComponent<PositionHandler>();
    }

    private void Start()
    {
        _ros = ROSConnection.GetOrCreateInstance();
    }

    private void Update()
    {
        UpdateInputs();

        if (_inputs[KeyCode.Tab])
            _mapControlMode = !_mapControlMode;

        if (_mapControlMode)
            HandleMapControl();
        else
            HandleRobotControl();
    }

    private void HandleMapControl()
    {
        Vector3 movement = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        if (_inputs[KeyCode.W]) movement.z += 1f;
        if (_inputs[KeyCode.S]) movement.z -= 1f;
        if (_inputs[KeyCode.A]) movement.x -= 1f;
        if (_inputs[KeyCode.D]) movement.x += 1f;
        if (_inputs[KeyCode.Space]) movement.y += 1f;
        if (_inputs[KeyCode.LeftShift]) movement.y -= 1f;

        if (_inputs[KeyCode.UpArrow]) rotation.x += 50f;
        if (_inputs[KeyCode.DownArrow]) rotation.x -= 50f;
        if (_inputs[KeyCode.LeftArrow]) rotation.y -= 50f;
        if (_inputs[KeyCode.RightArrow]) rotation.y += 50f;

        _positionHandler.UpdateTransform(movement, rotation);

        var zeroTwist = new TwistMsg();
        _ros.Publish(twistTopic, zeroTwist);
    }

    private void HandleRobotControl()
    {
        var msg = new TwistMsg
        {
            linear = new Vector3Msg(),
            angular = new Vector3Msg()
        };

        if (_inputs[KeyCode.W]) msg.linear.x += 1f;
        if (_inputs[KeyCode.S]) msg.linear.x -= 1f;
        if (_inputs[KeyCode.A]) msg.linear.y += 1f;
        if (_inputs[KeyCode.D]) msg.linear.y -= 1f;
        if (_inputs[KeyCode.Space]) msg.linear.z += 1f;
        if (_inputs[KeyCode.LeftShift]) msg.linear.z -= 1f;

        if (_inputs[KeyCode.LeftArrow]) msg.angular.z += 1f;
        if (_inputs[KeyCode.RightArrow]) msg.angular.z -= 1f;

        _ros.Publish(twistTopic, msg);
        _positionHandler.UpdateTransform(Vector3.zero, Vector3.zero);
    }

    private void UpdateInputs()
    {
        foreach (var key in _inputs.Keys.ToList())
        {
            if (key == KeyCode.Tab) continue;
            if (Input.GetKeyDown(key)) _inputs[key] = true;
            if (Input.GetKeyUp(key)) _inputs[key] = false;
        }
        _inputs[KeyCode.Tab] = Input.GetKeyDown(KeyCode.Tab);
    }
}
