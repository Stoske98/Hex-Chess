namespace Networking.Client
{
    public class NetStopMatchFinding : NetMessage
    {
        public NetStopMatchFinding()
        {
            Code = OpCode.STOP_MATCH_FINDING;
        }

        public NetStopMatchFinding(Packet reader)
        {
            Code = OpCode.STOP_MATCH_FINDING;
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
            RealtimeNetworking.C_STOP_MATCH_FINDING_RESPONESS?.Invoke(this);
        }
    }
}