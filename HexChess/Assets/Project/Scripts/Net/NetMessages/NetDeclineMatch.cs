namespace Networking.Client
{
    public class NetDeclineMatch : NetMessage
    {

        public ClassType class_type;
        public NetDeclineMatch()
        {
            Code = OpCode.DECLINE_MATCH;
        }

        public NetDeclineMatch(Packet reader)
        {
            Code = OpCode.DECLINE_MATCH;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(Packet reader)
        {
            class_type = (ClassType)reader.ReadByte();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_DECLINE_MATCH_RESPONESS?.Invoke(this);
        }
    }
}