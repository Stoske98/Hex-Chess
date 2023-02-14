namespace Networking.Client
{
    public enum Error
    {
    }
    public class NetError : NetMessage
    {
        public Error Error { set; get; }
        public NetError()
        {
            Code = OpCode.ERROR;
        }

        public NetError(Packet reader)
        {
            Code = OpCode.ERROR;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteByte((byte)Error);
        }

        public override void Deserialize(Packet reader)
        {
            Error = (Error)reader.ReadByte();
        }

        public override void ReceivedOnClient()
        {
            RealtimeNetworking.C_ERROR_RESPONESS?.Invoke(this);
        }
    }
}