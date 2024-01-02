using System.Collections.Generic;
using StarSquad.Loader;
using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Lobby.Party.Packet
{
    public class JoinPartyIncomingPacket : IncomingPacket
    {
        private enum JoinPartyResponse
        {
            Ok,
            PartyNotFound,
            PartyFull,
            NoInvite,
            Failed
        }

        private JoinPartyResponse response;
        private string partyCode;
        private string leaderId;
        private List<PartyMember> members;
        
        public void Read(ByteBuf buf)
        {
            this.response = (JoinPartyResponse)buf.ReadByte();
            if (this.response == JoinPartyResponse.Ok)
            {
                this.partyCode = buf.ReadString();

                this.leaderId = buf.ReadString();
                
                var count = buf.ReadByte();
                this.members = new List<PartyMember>(count);
                for (var i = 0; i < count; i++)
                {
                    var member = new PartyMember();
                    member.Read(buf);
                    this.members.Add(member);
                }
            }
        }

        public void Handle()
        {
            ScreenManager.GetScreenManager().party.ResetJoinButton();

            if (this.response != JoinPartyResponse.Ok)
            {
                Debug.Log("Failed to join party");

                string msg;
                switch (this.response)
                {
                    case JoinPartyResponse.PartyNotFound:
                        msg = "Can't find a party with the given code";
                        break;
                    
                    case JoinPartyResponse.PartyFull:
                        msg = "This party is full";
                        break;
                    
                    case JoinPartyResponse.NoInvite:
                        msg = "You don't have an invite from this party";
                        break;
                    
                    default:
                        msg = "Failed to join a party";
                        break;
                }
                
                LoaderManager.instance.alertManager.Alert(msg);
                
                return;
            }

            LobbyManager.instance.partyManager.JoinParty(this.partyCode);
            foreach (var member in this.members)
            {
                LobbyManager.instance.partyManager.OtherPlayerJoinParty(member, true, member.playerId == this.leaderId);
            }
        }
    }
}