using StarSquad.Loader;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Hero
{
    public class CancelUpgradeHeroIncomingPacket : IncomingPacket
    {
        private bool buy;

        public void Read(ByteBuf buf)
        {
            this.buy = buf.ReadBool();
        }

        public void Handle()
        {
            LoaderManager.instance.alertManager.Alert(this.buy ? "Failed to buy hero!" : "Failed to upgrade hero!");
        }
    }
}