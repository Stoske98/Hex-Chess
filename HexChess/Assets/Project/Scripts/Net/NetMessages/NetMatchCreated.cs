namespace Networking.Client
{
    public class NetMatchCreated : NetMessage
    {
        public int match_id;
        public string ip;
        public int port;
        public NetMatchCreated()
        {
            Code = OpCode.MATCH_CREATED;
        }

        public NetMatchCreated(Packet reader)
        {
            Code = OpCode.MATCH_CREATED;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(Packet reader)
        {
            match_id = reader.ReadInt();
            ip = reader.ReadString();
            port = reader.ReadInt();
        }


        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_MATCH_CREATED_RESPONESS?.Invoke(this);
        }
    }
}