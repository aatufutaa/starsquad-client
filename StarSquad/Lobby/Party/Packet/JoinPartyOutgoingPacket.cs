using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Lobby.Party.Packet
{
    public class JoinPartyOutgoingPacket : OutgoingPacket
    {
        private readonly string partyCode;

        public JoinPartyOutgoingPacket(string partyCode)
        {
            this.partyCode = partyCode;
        }
        
        public void Write(ByteBuf buf)
        {
            Debug.Log("write join party");
            buf.WriteString(this.partyCode);
        }
    }
}