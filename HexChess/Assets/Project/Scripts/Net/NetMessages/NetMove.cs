namespace Networking.Client
{
    public class NetMove : NetMessage
    {
        public int end_turn = 0;
        public int unit_id { set; get; }
        public Data.Hex to_hex { set; get; }
        public NetMove()
        {
            Code = OpCode.MOVE;
        }

        public NetMove(Packet reader)
        {
            Code = OpCode.MOVE;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(unit_id);
            writer.WriteInt(to_hex.column);
            writer.WriteInt(to_hex.row);
            writer.WriteInt(end_turn);

        }

        public override void Deserialize(Packet reader)
        {
            unit_id = reader.ReadInt();
            to_hex = new Data.Hex
            {
                column = reader.ReadInt(),
                row = reader.ReadInt(),
            };
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_MOVE_RESPONESS?.Invoke(this);
        }
    }
}