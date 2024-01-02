using StarSquad.Lobby.UI.Quests;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Quests
{
    public class QuestIncomingPacket : IncomingPacket
    {
        private int id;
        private int amount;
        private int step;

        public void Read(ByteBuf buf)
        {
            this.id = buf.ReadByte();
            this.amount = buf.ReadShort();
            this.step = buf.ReadByte();
        }

        public void Handle()
        {
            QuestsManager.Get().OnClaim(this.id, this.amount, this.step);
        }
    }
}