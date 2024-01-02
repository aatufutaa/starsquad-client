using StarSquad.Loader;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.HeroPass
{
    public class CancelBuyNextTierHeroPassIncomingPacket : IncomingPacket
    {
        public void Read(ByteBuf buf)
        {
        }

        public void Handle()
        {
            LoaderManager.instance.alertManager.Alert("Failed to buy next tier");
        }
    }
}