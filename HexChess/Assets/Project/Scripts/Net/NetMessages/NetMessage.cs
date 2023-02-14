namespace Networking.Client
{
    public class NetMessage
    {
        public OpCode Code { set; get; }

        public virtual void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }
        public virtual void Deserialize(Packet reader)
        {
        }
        public virtual void ReceivedOnClient()
        {

        }
    }
}