namespace Networking.Client
{
    public enum AbilityInput
    {
        None = -1,
        Q = 0,
        W = 1,
        E = 2,
        S = 3,
    }
    public class NetAbility : NetMessage
    {
        public int end_turn = 0;
        public AbilityInput input { set; get; }
        public int unit_id { set; get; }
        public Data.Hex to_hex { set; get; }
        public NetAbility()
        {
            Code = OpCode.ABILITY;
        }

        public NetAbility(Packet reader)
        {
            Code = OpCode.ABILITY;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteByte((byte)input);
            writer.WriteInt(unit_id);
            writer.WriteInt(to_hex.column);
            writer.WriteInt(to_hex.row);
            writer.WriteInt(end_turn);

        }

        public override void Deserialize(Packet reader)
        {
            input = (AbilityInput)reader.ReadByte();
            unit_id = reader.ReadInt();
            to_hex = new Data.Hex
            {
                column = reader.ReadInt(),
                row = reader.ReadInt(),
            };
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_ABILITY_RESPONESS?.Invoke(this);
        }
    }
}