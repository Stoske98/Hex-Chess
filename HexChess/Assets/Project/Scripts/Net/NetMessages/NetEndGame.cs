namespace Networking.Client
{
    public class NetEndGame : NetMessage
    {
        public ClassType class_win;
        public string ip;
        public int port;
        public NetEndGame()
        {
            Code = OpCode.END_GAME;
        }

        public NetEndGame(Packet reader)
        {
            Code = OpCode.END_GAME;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(Packet reader)
        {
            class_win = (ClassType)reader.ReadByte();
            ip = reader.ReadString();
            port = reader.ReadInt();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_END_GAME_RESPONESS?.Invoke(this);
        }
    }
}