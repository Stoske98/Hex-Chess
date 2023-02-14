namespace Networking.Client
{
    public class NetWelcomeGame : NetMessage
    {
        public NetWelcomeGame()
        {
            Code = OpCode.ON_WELCOME_GAME;
        }

        public NetWelcomeGame(Packet reader)
        {
            Code = OpCode.ON_WELCOME_GAME;
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
            RealtimeNetworking.C_WELCOME_GAME_RESPONESS?.Invoke(this);
        }
    }
}