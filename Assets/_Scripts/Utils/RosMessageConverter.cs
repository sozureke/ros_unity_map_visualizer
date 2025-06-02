using UnityEngine;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

public static class RosMessageConverter
{
    public static Vector3 PointMsgToUnity(PointMsg p) =>
        new Vector3(-(float)p.y, (float)p.z, (float)p.x);

    public static Vector3 Vector3MsgToUnity(Vector3Msg v) =>
        new Vector3(-(float)v.y, (float)v.z, (float)v.x);

    public static Color ColorMsgToUnity(ColorRGBAMsg c) =>
        new Color((float)c.r, (float)c.g, (float)c.b, (float)c.a);
}
