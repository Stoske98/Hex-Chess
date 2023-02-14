namespace Networking.Client
{
    public class NetChangeNickname : NetMessage
    {
        public string nickname { set; get; }
        public NetChangeNickname()
        {
            Code = OpCode.NICKNAME;
        }

        public NetChangeNickname(Packet reader)
        {
            Code = OpCode.NICKNAME;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(nickname);
        }

        public override void Deserialize(Packet reader)
        {
            nickname = reader.ReadString();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_CHANGE_NICKNAME_RESPONESS?.Invoke(this);
        }
    }

}