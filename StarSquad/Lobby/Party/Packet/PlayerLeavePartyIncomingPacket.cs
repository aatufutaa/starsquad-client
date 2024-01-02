using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Party.Packet
{
    public class PlayerLeavePartyIncomingPacket : IncomingPacket
    {
        private string playerId;
        
        public void Read(ByteBuf buf)
        {
            this.playerId = buf.ReadString();
        }

        public void Handle()
        {
            LobbyManager.instance.partyManager.OtherPlayerLeaveParty(this.playerId);
        }
    }
}