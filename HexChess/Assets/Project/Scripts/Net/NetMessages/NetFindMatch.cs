namespace Networking.Client
{
    public class NetFindMatch : NetMessage
    {
        public Data.Player enemy_data;
        public NetFindMatch()
        {
            Code = OpCode.FIND_MATCH;
        }

        public NetFindMatch(Packet reader)
        {
            Code = OpCode.FIND_MATCH;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(Packet reader)
        {
            enemy_data = new Data.Player
            {
                nickname = reader.ReadString(),
                rank = reader.ReadInt(),
                class_type = (ClassType)reader.ReadByte()
            };
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_FIND_MATCH_RESPONESS?.Invoke(this);
        }
    }
}