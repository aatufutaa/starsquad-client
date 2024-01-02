using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.HeroPass
{
    public class BuyHeroPassIncomingPacket : IncomingPacket
    {
        public void Read(ByteBuf buf)
        {
        }

        public void Handle()
        {
            ScreenManager.GetScreenManager().heroPass.OnHeroPassPurchase();
        }
    }
}