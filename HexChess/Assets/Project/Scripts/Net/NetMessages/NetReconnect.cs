namespace Networking.Client
{
    public class NetReconnect : NetMessage
    {
        public NetReconnect()
        {
            Code = OpCode.RECONNECT;
        }

        public NetReconnect(Packet reader)
        {
            Code = OpCode.RECONNECT;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_RECONNECT_RESPONESS?.Invoke(this);
        }
    }

}