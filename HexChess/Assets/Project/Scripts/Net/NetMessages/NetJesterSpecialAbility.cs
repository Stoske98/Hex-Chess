namespace Networking.Client
{
    public class NetJesterSpecialAbility : NetMessage
    {
        public Data.Unit unit { get; set; }
        public Data.Hex to_hex { set; get; }
        public NetJesterSpecialAbility()
        {
            Code = OpCode.JESTER_SA;
        }

        public NetJesterSpecialAbility(Packet reader)
        {
            Code = OpCode.JESTER_SA;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(Packet reader)
        {
            unit = Data.Deserialize<Data.Unit>(reader.ReadString());

            to_hex = new Data.Hex
            {
                column = reader.ReadInt(),
                row = reader.ReadInt(),
            };
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_JESTER_SA_RESPONESS?.Invoke(this);
        }
    }
}