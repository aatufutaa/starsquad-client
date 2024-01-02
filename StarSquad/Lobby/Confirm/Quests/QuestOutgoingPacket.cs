using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Quests
{
    public class QuestOutgoingPacket : OutgoingPacket
    {
        private readonly int id;
        private readonly int step;

        public QuestOutgoingPacket(int id, int step)
        {
            this.id = id;
            this.step = step;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteByte((byte)this.id);
            buf.WriteByte((byte)this.step);
        }
    }
}