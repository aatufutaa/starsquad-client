using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Party.Packet
{
    public class PlayerJoinPartyIncomingPacket : IncomingPacket
    {
        private PartyMember partyMember;

        public void Read(ByteBuf buf)
        {
            this.partyMember = new PartyMember();
            this.partyMember.Read(buf);
        }

        public void Handle()
        {
            LobbyManager.instance.partyManager.OtherPlayerJoinParty(this.partyMember, false, false);
        }
    }
}