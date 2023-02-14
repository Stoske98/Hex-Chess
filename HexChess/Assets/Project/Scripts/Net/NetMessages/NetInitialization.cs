namespace Networking.Client
{
    public class NetInitialization : NetMessage
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public NetInitialization()
        {
            Code = OpCode.INIT;
        }

        public NetInitialization(Packet reader)
        {
            Code = OpCode.INIT;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(Token);
        }

        public override void Deserialize(Packet reader)
        {
            Id = reader.ReadInt();
            Token = reader.ReadString();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_INIT_RESPONESS?.Invoke(this);
        }
    }

}