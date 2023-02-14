namespace Networking.Client
{
    public class NetEndTurn : NetMessage
    {
        public ClassType class_on_turn;
        public NetEndTurn()
        {
            Code = OpCode.END_TURN;
        }

        public NetEndTurn(Packet reader)
        {
            Code = OpCode.END_TURN;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);

        }

        public override void Deserialize(Packet reader)
        {
            class_on_turn = (ClassType)reader.ReadByte();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_END_TURN_RESPONESS?.Invoke(this);
        }
    }
}