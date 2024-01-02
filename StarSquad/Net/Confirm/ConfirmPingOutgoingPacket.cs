using StarSquad.Net.Packet;

namespace StarSquad.Net.Confirm
{
    public class ConfirmPingOutgoingPacket : OutgoingPacket
    {
        private readonly int latestAcceptedId;

        public ConfirmPingOutgoingPacket(int latestAcceptedId)
        {
            this.latestAcceptedId = latestAcceptedId;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteInt(this.latestAcceptedId);
        }
    }
}