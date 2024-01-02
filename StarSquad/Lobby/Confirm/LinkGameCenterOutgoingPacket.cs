using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm
{
    public class LinkGameCenterOutgoingPacket : OutgoingPacket
    {
        private readonly string id;

        public LinkGameCenterOutgoingPacket(string id)
        {
            this.id = id;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteString(this.id);
        }
    }
}