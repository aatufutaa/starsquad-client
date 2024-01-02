using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Screens;

namespace StarSquad.Net.Packet.Lobby.Queue
{
    public class QueueStatusIncomingPacket : IncomingPacket
    {
        private int players;
        private int maxPlayers;
        private bool party;

        public void Read(ByteBuf buf)
        {
            this.players = buf.ReadByte();
            this.maxPlayers = buf.ReadByte();
            this.party = buf.ReadBool();
        }

        public void Handle()
        {
            // show queue if this is party member (or reconnect?)
            var queue = ScreenManager.GetScreenManager().queue;
            queue.Show();
  
            queue.UpdatePlayerCount(this.players, this.maxPlayers);
            
            // remove leave button if party (this is false for leader)
            if (this.party)
            {
                queue.SetParty();
            }
        }
    }
}