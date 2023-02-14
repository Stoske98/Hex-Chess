namespace Networking.Client
{
    public class NetSurrend : NetMessage
    {
        public NetSurrend()
        {
            Code = OpCode.SURREND;
        }

        public NetSurrend(Packet reader)
        {
            Code = OpCode.SURREND;
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
            RealtimeNetworking.C_SURREND_RESPONESS?.Invoke(this);
        }
    }

}