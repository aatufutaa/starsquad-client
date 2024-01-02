using StarSquad.Game;

namespace StarSquad.Net.Packet.Game.Misc
{
    public class GameResultIncomingPacket : IncomingPacket
    {
        private int place;
        private int kills;
        private int giveTrophies;
        private int giveTokens;
        
        public void Read(ByteBuf buf)
        {
            this.place = buf.ReadByte();
            this.kills = buf.ReadByte();
            this.giveTrophies = buf.ReadByte();
            this.giveTokens = buf.ReadByte();
        }

        public void Handle()
        {
            GameManager.instance.exitButton.gameObject.SetActive(true);
            GameManager.instance.gameResultsUI.SetResults(this.place, this.kills, this.giveTrophies, this.giveTokens);
        }
    }
}