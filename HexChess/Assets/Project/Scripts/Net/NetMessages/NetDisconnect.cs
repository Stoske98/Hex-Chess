namespace Networking.Client
{
    public class NetDisconnect : NetMessage
    {
        public string nickname { set; get; }
        public NetDisconnect()
        {
            Code = OpCode.DISCONNECT;
        }

        public NetDisconnect(Packet reader)
        {
            Code = OpCode.DISCONNECT;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(Packet reader)
        {
            nickname = reader.ReadString();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_DISCONNECT_RESPONESS?.Invoke(this);
        }
    }

}