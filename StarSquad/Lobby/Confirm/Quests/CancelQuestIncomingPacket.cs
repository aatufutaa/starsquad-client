using StarSquad.Loader;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Quests
{
    public class CancelQuestIncomingPacket : IncomingPacket
    {
        public void Read(ByteBuf buf)
        {
        }

        public void Handle()
        {
            LoaderManager.instance.alertManager.Alert("Failed to claim quest reward");
        }
    }
}