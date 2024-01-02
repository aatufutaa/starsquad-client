using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Misc
{
    public class ClaimLevelRewardIncomingPacket : IncomingPacket
    {
        private int level;

        public void Read(ByteBuf buf)
        {
            this.level = buf.ReadByte();
        }

        public void Handle()
        {
            ScreenManager.GetScreenManager().level.UpdateClaimIndex(this.level);
        }
    }
}