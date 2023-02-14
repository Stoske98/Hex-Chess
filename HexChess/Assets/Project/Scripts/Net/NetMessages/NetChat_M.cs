namespace Networking.Client
{
    public class NetChat_M : NetMessage
    {
        public string nickname { get; set; }
        public string message { get; set; }
        public NetChat_M()
        {
            Code = OpCode.CHAT_M;
        }

        public NetChat_M(Packet reader)
        {
            Code = OpCode.CHAT_M;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(message);
        }

        public override void Deserialize(Packet reader)
        {
            nickname = reader.ReadString();
            message = reader.ReadString();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_CHAT_M_RESPONESS?.Invoke(this);
        }
    }

    public class NetChat : NetMessage
    {
        public string nickname { get; set; }
        public string message { get; set; }
        public NetChat()
        {
            Code = OpCode.CHAT;
        }

        public NetChat(Packet reader)
        {
            Code = OpCode.CHAT;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(message);
        }

        public override void Deserialize(Packet reader)
        {
            nickname = reader.ReadString();
            message = reader.ReadString();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_CHAT_RESPONESS?.Invoke(this);
        }
    }

}