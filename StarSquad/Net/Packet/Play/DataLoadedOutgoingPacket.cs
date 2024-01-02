namespace StarSquad.Net.Packet.Play
{
    public class DataLoadedOutgoingPacket : OutgoingPacket
    {
        private readonly bool send;
        private readonly int latestAcceptedId;

        public DataLoadedOutgoingPacket(int latestAcceptedId)
        {
            this.send = true;
            this.latestAcceptedId = latestAcceptedId;
        }

        public DataLoadedOutgoingPacket()
        {
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteBool(this.send);
            if (this.send)
                buf.WriteInt(this.latestAcceptedId);
        }
    }
}