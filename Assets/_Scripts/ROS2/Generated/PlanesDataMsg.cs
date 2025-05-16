//using Unity.Robotics.ROSTCPConnector.MessageGeneration;
//using RosMessageTypes.Std;
//using RosMessageTypes.Geometry;

//public class PlanesDataMsg : Message
//{
//    public const string RosMessageName = "lidar_situational_graphs/PlanesData";

//    public HeaderMsg header = new HeaderMsg();
//    public int id;
//    public PoseMsg floor_center = new PoseMsg();
//    public int[] keyframe_ids = System.Array.Empty<int>();

//    public override string RosMessageName => RosMessageName;
//    public override void SerializeTo(MessageSerializer ser)
//    {
//        header.SerializeTo(ser);
//        ser.Write(id);
//        floor_center.SerializeTo(ser);
//        ser.Write(keyframe_ids, (s, v) => s.Write(v));
//    }
//    public override void Deserialize(MessageDeserializer des)
//    {
//        header.Deserialize(des);
//        id = des.Read<int>();
//        floor_center.Deserialize(des);
//        keyframe_ids = des.ReadInt32Array();
//    }
//}
