namespace Networking.Client
{
    public class NetAttack : NetMessage
    {
        public int end_turn = 0;
        public int unit_id { set; get; }
        public int enemy_id { set; get; }
        public NetAttack()
        {
            Code = OpCode.ATTACK;
        }

        public NetAttack(Packet reader)
        {
            Code = OpCode.ATTACK;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(unit_id);
            writer.WriteInt(enemy_id);
            writer.WriteInt(end_turn);

        }

        public override void Deserialize(Packet reader)
        {
            unit_id = reader.ReadInt();
            enemy_id = reader.ReadInt();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_ATTACK_RESPONESS?.Invoke(this);
        }
    }
}