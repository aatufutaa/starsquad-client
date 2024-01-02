using StarSquad.Lobby.UI.Heroes;
using StarSquad.Lobby.UI.Panel;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Hero
{
    public class UpgradeHeroIncomingPacket : IncomingPacket
    {
        private int id;
        private int level;

        public void Read(ByteBuf buf)
        {
            this.id = buf.ReadByte();
            this.level = buf.ReadByte();
        }

        public void Handle()
        {
            if (this.level == 0)
            {
                HeroManager.Get().OnHeroUnlocked(this.id, 0, 1);

                ScreenManager.GetScreenManager().hero.SetValue(this.id, true, 0, 1);
            }
            else
            {
                HeroManager.Get().OnUpgrade(this.id, this.level);
                ScreenManager.GetScreenManager().hero.SetLevel(this.id, this.level, true, true);

                PanelManager.Get().upgrade.Close();
            }
        }
    }
}