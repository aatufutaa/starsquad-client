using StarSquad.Loader;
using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Lobby.Party.Packet
{
    public class CreatePartyIncomingPacket : IncomingPacket
    {
        private enum CreatePartyResponse
        {
            Ok,
            Failed
        }

        private CreatePartyResponse response;
        private string partyCode;

        public void Read(ByteBuf buf)
        {
            this.response = (CreatePartyResponse)buf.ReadByte();
            if (this.response == CreatePartyResponse.Ok)
            {
                this.partyCode = buf.ReadString();
            }
        }

        public void Handle()
        {
            ScreenManager.GetScreenManager().party.ResetCreateButton();

            if (this.response != CreatePartyResponse.Ok)
            {
                Debug.Log("Failed to create party");
                
                LoaderManager.instance.alertManager.Alert("Failed to create a party");
                return;
            }
            
            LobbyManager.instance.partyManager.CreateParty(this.partyCode, false, true);
        }
    }
}