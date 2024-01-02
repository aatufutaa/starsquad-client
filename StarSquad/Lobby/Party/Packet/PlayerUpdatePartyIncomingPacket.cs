using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Party.Packet
{
    public class PlayerUpdatePartyIncomingPacket : IncomingPacket
    {
        private PartyMember partyMember;

        public void Read(ByteBuf buf)
        {
            this.partyMember = new PartyMember();
            this.partyMember.Read(buf);
        }

        public void Handle()
        {
            LobbyManager.instance.partyManager.OtherPlayerUpdateParty(this.partyMember);
        }
    }
}