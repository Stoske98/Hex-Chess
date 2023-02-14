namespace Networking.Client
{
    public class NetSynchronic : NetMessage
    {
        public Data.Game game_data { get; set; }
        public NetSynchronic()
        {
            Code = OpCode.SYNC;
        }

        public NetSynchronic(Packet reader)
        {
            Code = OpCode.SYNC;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(Packet reader)
        {
            game_data = Data.Deserialize<Data.Game>(reader.ReadString());
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_SYNC_RESPONESS?.Invoke(this);
        }

    }
    public class NetSynchronicM : NetMessage
    {
        public int match_id { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public NetSynchronicM()
        {
            Code = OpCode.SYNC_M;
        }

        public NetSynchronicM(Packet reader)
        {
            Code = OpCode.SYNC_M;
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
            RealtimeNetworking.C_SYNCHRONIC_RESPONESS?.Invoke(this);
        }

    }
}