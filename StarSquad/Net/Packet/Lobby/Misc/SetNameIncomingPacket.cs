using StarSquad.Loader;
using StarSquad.Lobby;
using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Screens;
using UnityEngine;

namespace StarSquad.Net.Packet.Lobby.Misc
{
    public class SetNameIncomingPacket : IncomingPacket
    {
        public enum NameResponse
        {
            Ok,
            NameChanged,
            NotAllowed,
            InvalidCharacters,
            InvalidOther,
            UpdateFail
        }

        private NameResponse response;
        private string name;

        public void Read(ByteBuf buf)
        {
            this.response = (NameResponse)buf.ReadByte();
            if (this.response == NameResponse.Ok || this.response == NameResponse.NameChanged)
            {
                this.name = buf.ReadString();
            }
        }

        public void Handle()
        {
            Debug.Log("name set response " + this.response);
            
            if (this.response == NameResponse.Ok)
            {
                ScreenManager.GetScreenManager().nameScreen.Hide();
                LobbyManager.instance.lobbyData.OnNameChanged(this.name);
                return;
            }

            if (this.response == NameResponse.NameChanged)
            {
                // TODO:
                LoaderManager.instance.alertManager.Alert("Your name has been changed to " + this.name);
                LobbyManager.instance.lobbyData.OnNameChanged(this.name);
                return;
            }
            
            string msg;
            switch (this.response)
            {
                case NameResponse.NotAllowed:
                    msg = "This name is not allowed. Please try another one.";
                    break;

                case NameResponse.InvalidCharacters:
                    msg = "This name contains disallowed characters.";
                    break;

                case NameResponse.InvalidOther:
                    msg = "This name is invalid.";
                    break;

                case NameResponse.UpdateFail:
                    msg = "Failed to update your name.";
                    break;

                default:
                    msg = "Don't know what to say with response " + this.response;
                    break;
            }

            LoaderManager.instance.alertManager.Alert(msg);
        }
    }
}