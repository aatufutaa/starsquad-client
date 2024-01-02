using StarSquad.Loader;
using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Lobby.Party.Packet
{
    public class LeavePartyIncomingPacket : IncomingPacket
    {
        private enum PartyLeaveResponse
        {
            Ok,
            Disband,
            NotInParty,
            CantLeave
        }

        private PartyLeaveResponse response;
        
        public void Read(ByteBuf buf)
        {
            this.response = (PartyLeaveResponse)buf.ReadByte();
        }

        public void Handle()
        {
            ScreenManager.GetScreenManager().party.ResetLeaveButton();

            if (this.response != PartyLeaveResponse.Ok && this.response != PartyLeaveResponse.Disband)
            {
                Debug.Log("Failed to leave party");

                string msg;
                switch (this.response)
                {
                    case PartyLeaveResponse.NotInParty:
                        msg = "You are not in a party";
                        break;
                    
                    case PartyLeaveResponse.CantLeave:
                        msg = "You can't leave the party";
                        break;
                    
                    default:
                        msg = "Failed to leave the party";
                        break;
                }
                
                LoaderManager.instance.alertManager.Alert(msg);
                
                return;
            }

            LobbyManager.instance.partyManager.LeaveParty(this.response == PartyLeaveResponse.Ok);
        }
    }
}