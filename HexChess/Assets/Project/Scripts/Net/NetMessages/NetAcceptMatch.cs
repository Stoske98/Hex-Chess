namespace Networking.Client
{
    public class NetAcceptMatch : NetMessage
    {
        public ClassType class_type;
        public string xml_team_structure;
        public NetAcceptMatch()
        {
            Code = OpCode.ACCEPT_MATCH;
        }

        public NetAcceptMatch(Packet reader)
        {
            Code = OpCode.ACCEPT_MATCH;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(xml_team_structure);
        }

        public override void Deserialize(Packet reader)
        {
            class_type = (ClassType)reader.ReadByte();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_ACCEPT_MATCH_RESPONESS?.Invoke(this);
        }
    }
}