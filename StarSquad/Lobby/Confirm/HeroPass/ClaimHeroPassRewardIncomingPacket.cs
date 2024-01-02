using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.HeroPass
{
    public class ClaimHeroPassRewardIncomingPacket : IncomingPacket
    {
        private int id;
        private bool hero;

        public void Read(ByteBuf buf)
        {
            this.id = buf.ReadByte();
            this.hero = buf.ReadBool();
        }

        public void Handle()
        {
            ScreenManager.GetScreenManager().heroPass.OnClaimReward(this.id, this.hero);
        }
    }
}