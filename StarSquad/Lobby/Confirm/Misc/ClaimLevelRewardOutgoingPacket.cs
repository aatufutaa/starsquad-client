using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Misc
{
    public class ClaimLevelRewardOutgoingPacket : OutgoingPacket
    {
        private readonly int id;
        private readonly int level;

        public ClaimLevelRewardOutgoingPacket(int id, int level)
        {
            this.id = id;
            this.level = level;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteShort((short)this.id);
            buf.WriteByte((byte)this.level);
        }
    }
}