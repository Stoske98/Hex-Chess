namespace Networking.Client
{
    public class NetAuthentication_M : NetMessage
    {
        public string deviceID { get; set; }
        public Data.Player player { get; set; }
        public NetAuthentication_M()
        {
            Code = OpCode.AUTHENTICATION_M;
        }

        public NetAuthentication_M(Packet reader)
        {
            Code = OpCode.AUTHENTICATION_M;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(deviceID);
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
            RealtimeNetworking.C_AUTHENTICATION_RESPONESS?.Invoke(this);
        }
    }


}