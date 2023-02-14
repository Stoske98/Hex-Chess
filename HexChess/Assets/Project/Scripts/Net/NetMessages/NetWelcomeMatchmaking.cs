namespace Networking.Client
{
    public class NetWelcomeMatchmaking : NetMessage
    {
        public NetWelcomeMatchmaking()
        {
            Code = OpCode.ON_WELCOME_MATCHMAKING;
        }

        public NetWelcomeMatchmaking(Packet reader)
        {
            Code = OpCode.ON_WELCOME_MATCHMAKING;
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
            RealtimeNetworking.C_WELCOME_MATCHMAKING_RESPONESS?.Invoke(this);
        }
    }
}