using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Misc
{
    public class ClaimProgressionOutgoingPacket : OutgoingPacket
    {
        private readonly int id;
        private readonly int progression;

        public ClaimProgressionOutgoingPacket(int id, int progression)
        {
            this.id = id;
            this.progression = progression;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteShort((short)this.id);
            buf.WriteByte((byte)this.progression);
        }
    }
}