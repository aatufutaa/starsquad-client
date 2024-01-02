using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Misc
{
    public class ClaimProgressionIncomingPacket : IncomingPacket
    {
        private int id;

        public void Read(ByteBuf buf)
        {
            this.id = buf.ReadByte();
        }

        public void Handle()
        {
            ScreenManager.GetScreenManager().progression.OnClaim(this.id);
        }
    }
}