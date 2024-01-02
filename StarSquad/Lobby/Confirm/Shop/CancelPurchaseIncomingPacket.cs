using StarSquad.Loader;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Shop
{
    public class CancelPurchaseIncomingPacket : IncomingPacket
    {
        public void Read(ByteBuf buf)
        {
        }

        public void Handle()
        {
            LoaderManager.instance.alertManager.Alert("Failed to purchase item!");
        }
    }
}