using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Misc
{
    public class RequestProfileIncomingPacket : IncomingPacket
    {
        private string name;
        private int rating;
        
        public void Read(ByteBuf buf)
        {
            this.name = buf.ReadString();

            this.rating = buf.ReadInt();
        }

        public void Handle()
        {
            var profile = ScreenManager.GetScreenManager().profile;
            
            profile.SetName(this.name);
            
            profile.SetRating(this.rating);
            
            
        }
    }
}