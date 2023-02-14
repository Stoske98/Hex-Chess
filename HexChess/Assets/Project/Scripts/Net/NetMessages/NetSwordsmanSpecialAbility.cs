namespace Networking.Client
{
    public class NetSwordsmanSpecialAbility : NetMessage
    {
        public int unit_id { set; get; }
        public int enemy_id { set; get; }
        public NetSwordsmanSpecialAbility()
        {
            Code = OpCode.SWORDSMAN_SA;
        }

        public NetSwordsmanSpecialAbility(Packet reader)
        {
            Code = OpCode.SWORDSMAN_SA;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(unit_id);
            writer.WriteInt(enemy_id);

        }

        public override void Deserialize(Packet reader)
        {
            unit_id = reader.ReadInt();
            enemy_id = reader.ReadInt();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_SWORDSMAN_SA_RESPONESS?.Invoke(this);
        }
    }
}