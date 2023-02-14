namespace Networking.Client
{
    public class NetAuthentication : NetMessage
    {
        public string deviceID { get; set; }
        public int matchID { get; set; }
        public Data.Player player { get; set; }
        public NetAuthentication()
        {
            Code = OpCode.AUTH;
        }

        public NetAuthentication(Packet reader)
        {
            Code = OpCode.AUTH;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(deviceID);
            writer.WriteInt(matchID);
        }

        public override void Deserialize(Packet reader)
        {
            player = new Data.Player
            {
                nickname = reader.ReadString(),
                rank = reader.ReadInt(),
                class_type = (ClassType)reader.ReadByte()
            };
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_AUTH_RESPONESS?.Invoke(this);
        }
    }
}