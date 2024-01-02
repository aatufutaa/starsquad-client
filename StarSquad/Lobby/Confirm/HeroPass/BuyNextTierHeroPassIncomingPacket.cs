using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.HeroPass
{
    public class BuyNextTierHeroPassIncomingPacket : IncomingPacket
    {
        private int id;

        public void Read(ByteBuf buf)
        {
            this.id = buf.ReadByte();
        }

        public void Handle()
        {
            ScreenManager.GetScreenManager().heroPass.OnUnlockNext(this.id);
        }
    }
}