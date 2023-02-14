namespace Networking.Client
{
    public class NetCreateTicket : NetMessage
    {
        public ClassType class_type;
        public NetCreateTicket()
        {
            Code = OpCode.CREATE_TICKET;
        }

        public NetCreateTicket(Packet reader)
        {
            Code = OpCode.CREATE_TICKET;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteByte((byte)class_type);
        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_CREATE_TICKET_RESPONESS?.Invoke(this);
        }
    }

}